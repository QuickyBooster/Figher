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

	Vector3 playerPosition = Vector3.down;

	[Range(0, 360)]
	public float rangeVision;
	[Range(0, 10)]
	public float rangeAttack;
	bool playerInVision = false, playerInRange = false;

	bool walkPointset, alreadyAttack;
	public float walkPointRange, timeBetweenAttack;
	Vector3 walkPoint;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}




	private void Start()
	{
		StartCoroutine("FindTargetWithDelay", .5f);
	}
	private void Update()
	{
		if (playerPosition!=Vector3.down) playerInVision = true;
		else playerInVision= false;
		Debug.Log(playerInVision.ToString() + playerInRange);
		if (!playerInVision && !playerInRange) Patrolling();
		if (playerInVision && !playerInRange) ChasePlayer();
		if (playerInVision && playerInRange) AttackingPlayer();
	}
	IEnumerator FindTargetWithDelay(float delay)
	{
		while (true)
		{
			Debug.Log("finding target");
			yield return new WaitForSeconds(delay);
			playerPosition = fov.FindVisibleTarget();
		}
	}

	void Patrolling()
	{
		Debug.Log("patrollingg");
		if (!walkPointset) SearchWalkPoint();
		else agent.SetDestination(walkPoint);
		Vector3 distanceToWalkPoint = transform.position-walkPoint;

		if (distanceToWalkPoint.magnitude < 2f)
			walkPointset = false;
	}
	void SearchWalkPoint()
	{
		Debug.Log("search walk");
		float z = Random.Range(-walkPointRange, walkPointRange);
		float x = Random.Range(-walkPointRange, walkPointRange);
		walkPoint = new Vector3(transform.position.x+ x, transform.position.y, transform.position.z+z);
		if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
			walkPointset = true;
	}
	void AttackingPlayer()
	{
		Debug.Log("attack");
		agent.SetDestination(playerPosition);
		transform.LookAt(playerPosition);
		if (!alreadyAttack)
		{
			// attack player here
			alreadyAttack = true;
			Invoke(nameof(ResetAttack), timeBetweenAttack);
		}
	}
	void ResetAttack()
	{
		alreadyAttack= false;
	}

	void ChasePlayer()
	{
		agent.SetDestination(playerPosition);
	}
}
