using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
	NavMeshAgent agent;
	public FieldOfView fov;
	public LayerMask whatIsGround;

	[SerializeField]
	PlayerManager playerManager;
	[SerializeField]
	CharacterStats myStats;
	public CharacterStats MyStats => myStats;
	[SerializeField]
	CharacterCombat myCombat;

	[SerializeField]
	float moveSpeed;

	Vector3 playerPosition = new Vector3(100, 100, 100);
	Vector3 nonPosition = new Vector3(100, 100, 100);
	[Range(0, 360)]
	public float rangeVision;
	float defaultVision;
	[Range(0, 10)]
	public float rangeAttack;
	bool playerInVision = false, playerInRangeAttack = false, lostVision = true;
	float closeLimit;
	bool walkPointset, alreadyAttack;
	public float walkPointRange, timeBetweenAttack;
	Vector3 walkPoint;
	Animator animator;



	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.angularSpeed = rangeVision/2;
		fov.viewAngle = rangeVision;
		agent.speed = moveSpeed;
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		animator.fireEvents = false;
		playerManager = PlayerManager.instance;
		StartCoroutine("FindTargetWithDelay", .5f);
		if (myStats == null) myStats = GetComponent<CharacterStats>();
		if (myCombat == null) myCombat = GetComponent<CharacterCombat>();
		closeLimit = 0.5f;
		defaultVision = rangeVision;
	}
	private void Update()
	{

		if (!playerInVision && lostVision) Patrolling();
		if (playerInVision && playerInRangeAttack) AttackingPlayer();
		if (!lostVision && !playerInRangeAttack) ChasePlayer();
		//Debug.Log($"player in vision {playerInVision}- player in range: {playerInRange}");
		//if (!playerInVision) playerInRange = false;
		////Debug.Log(playerInVision.ToString() + playerInRange);
		//if (!playerInVision && !playerInRange && playerPosition == nonPosition) Patrolling();
		//if (!playerInVision && !playerInRange) ChasePlayer();
		//if (playerInVision && playerInRange) AttackingPlayer();
	}
	IEnumerator FindTargetWithDelay(float delay)
	{
		while (true)
		{
			Debug.Log("finding target");
			yield return new WaitForSeconds(delay);
			if (!playerInVision) playerInRangeAttack = false;
			Vector3 a = fov.FindVisibleTarget();
			if (a != new Vector3(100, 100, 100))
			{
				playerPosition = a;
				playerInVision = true;
				lostVision = false;
				Debug.Log("found target");
			} else
			{
				playerInVision = false;
				Debug.Log("target not found");
			}
		}
	}

	void Patrolling()
	{
		animator.Play("Walk");
		if (!walkPointset) SearchWalkPoint();
		else
		{
			agent.SetDestination(walkPoint);
		}
		Vector3 distanceToWalkPoint = transform.position - walkPoint;

		if (distanceToWalkPoint.magnitude < closeLimit*2.5f)
		{
			Debug.Log("walkPointSet = False");
			walkPointset = false;
		}
	}

	// Version 1: not included the enemy field of view
	//void SearchWalkPoint()
	//{
	//    Debug.Log("search walk");
	//    float z = Random.Range(-walkPointRange, walkPointRange);
	//    float x = Random.Range(-walkPointRange, walkPointRange);
	//    walkPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
	//    if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
	//        walkPointset = true;
	//}

	// Version 2: adjust the code so the walkPoint is in the enemy field of vision so he can't go backward
	void SearchWalkPoint()
	{
		Debug.Log("search walk");
		float z = Random.Range(-walkPointRange, walkPointRange);
		float x = Random.Range(-walkPointRange, walkPointRange);
		Vector3 potentialWalkPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

		// Calculate the direction from the player to the potential walk point
		Vector3 directionToWalkPoint = (potentialWalkPoint - transform.position).normalized;

		// Calculate the angle between the player's forward vector and the direction to the walk point
		float angleToWalkPoint = Vector3.Angle(transform.forward, directionToWalkPoint);

		// Check if the angle is within the player's field of view (70 degrees in this example)
		if (angleToWalkPoint <= rangeVision)
			if (Physics.Raycast(potentialWalkPoint, -transform.up, 2f, whatIsGround))
			{
				walkPoint = potentialWalkPoint;
				walkPointset = true;
			} else
				Debug.Log("Walk point is outside the player's field of view.");
	}

	bool attackLeft = true;
	void AttackingPlayer()
	{
		rangeVision = 360;
		fov.viewAngle = rangeVision;
		Debug.Log("attack");
		if (Vector3.Magnitude(transform.position -  playerPosition) > closeLimit)
			agent.SetDestination(playerPosition);
		transform.LookAt(playerPosition);
		if (attackLeft)
		{

			animator.Play("AttackLeft");
			attackLeft = false;
		} else
		{

			attackLeft = true;
			animator.Play("AttackRight");
		}
		if (!alreadyAttack)
		{
			// attack player here
			myCombat.Attack(playerManager.player.myStats);
			alreadyAttack = true;
			Invoke(nameof(ResetAttack), timeBetweenAttack);
		}
		if (!playerInVision)
			lostVision = true;
		Debug.Log("Lost vision when attacking: "+lostVision);

	}
	void ResetAttack()
	{
		alreadyAttack = false;
	}

	void ChasePlayer()
	{
		rangeVision = defaultVision;
		fov.viewAngle = rangeVision;
		agent.speed = moveSpeed*1.3f;
		animator.Play("Chase");
		Debug.Log("chasing player");
		agent.SetDestination(playerPosition);
		walkPointset = false;
		if (playerInVision)
			if (Vector3.Magnitude(playerPosition - transform.position) < rangeAttack)
				playerInRangeAttack = true;
			else playerInRangeAttack = false;
		else
			if (Vector3.Magnitude(playerPosition - transform.position) < closeLimit) lostVision = true;
	}
}
