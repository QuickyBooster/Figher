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

    Vector3 playerPosition = new Vector3(100, 100, 100);
    Vector3 nonPosition = new Vector3(100, 100, 100);
    [Range(0, 360)]
    public float rangeVision;
    [Range(0, 10)]
    public float rangeAttack;
    bool playerInVision = false, playerInRange = false, lostVision = true;

    bool walkPointset, alreadyAttack;
    public float walkPointRange, timeBetweenAttack;
    Vector3 walkPoint;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;
        StartCoroutine("FindTargetWithDelay", .5f);
        if (myStats == null) myStats = GetComponent<CharacterStats>();
        if (myCombat == null) myCombat = GetComponent<CharacterCombat>();
    }
    private void Update()
    {

        if (!playerInVision && lostVision) Patrolling();
        if (playerInVision && playerInRange) AttackingPlayer();
        if (!lostVision && !playerInRange) ChasePlayer();
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
            var a = fov.FindVisibleTarget();
            if (a != new Vector3(100, 100, 100))
            {
                playerPosition = a;
                playerInVision = true;
                lostVision = false;
            }
            else
            {
                playerInVision = false;
                lostVision = true;
            }
        }
    }

    void Patrolling()
    {
        Debug.Log($"patrollingg {walkPointset}");
        if (!walkPointset) SearchWalkPoint();
        else
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 2f)
        {
            Debug.Log("walkPointSet = False");
            walkPointset = false;
        }
    }
    void SearchWalkPoint()
    {
        Debug.Log("search walk");
        float z = Random.Range(-walkPointRange, walkPointRange);
        float x = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
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
            myCombat.Attack(playerManager.player.myStats);
            alreadyAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }
    }
    void ResetAttack()
    {
        alreadyAttack = false;
    }

    void ChasePlayer()
    {
        Debug.Log("chasing player");
        agent.SetDestination(playerPosition);
        if (playerInVision)
        {
            if (Vector3.Magnitude(playerPosition - transform.position) < rangeAttack)
                playerInRange = true;
            else playerInRange = false;
        } 
        else
        {
            if (Vector3.Magnitude(playerPosition - transform.position) < rangeAttack)
            {
                playerInVision = false;
                lostVision = true;
            }
        }

    }
}
