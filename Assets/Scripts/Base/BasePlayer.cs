using Assets.Scripts;
using System;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{

	float rotateSpeed { get; set; }
	float moveSpeed { get; set; }
    [SerializeField]
	public CharacterStats myStats { get; set; }
    [SerializeField]
	CharacterCombat myCombat { get; set; }
    float lineOfSight { get; set; }
	Weapon weapon { get; set; }
    [Range(0, 10)]
    public float attackRange;
    public BasePlayer(float rotate, float move, float sight, int hp)
	{
		rotateSpeed = rotate;
		moveSpeed = move;
		lineOfSight = sight;
    }
	void Start()
	{
		rotateSpeed = 300;
		moveSpeed = 4;
		lineOfSight = 2;
        if (myStats == null) myStats = GetComponent<CharacterStats>();
        if (myCombat == null) myCombat = GetComponent<CharacterCombat>();
    }

	void Update()
	{
		Moving(Time.deltaTime);
		Rotating();
	}

	void Moving(float deltaTime)
	{
		if (Input.GetAxisRaw("Horizontal") == 0)
		{
			if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(Mathf.PI/2, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-Mathf.PI/2, Time.deltaTime);
		} else if (Input.GetAxisRaw("Horizontal") > 0)
		{
			if (Input.GetAxisRaw("Vertical") == 0)
				OnMoving(0, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(Mathf.PI/4, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-Mathf.PI/4, Time.deltaTime);
		} else if (Input.GetAxisRaw("Horizontal") < 0)
		{
			if (Input.GetAxisRaw("Vertical") == 0)
				OnMoving(Mathf.PI, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(3*Mathf.PI/4, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-3*Mathf.PI/4, Time.deltaTime);
		}
	}
	public void OnMoving(float alpha, float deltaTime)
	{
		Vector3 start = transform.position;
		Vector3 end = new Vector3(
			transform.position.x + (float)Mathf.Cos(alpha)* moveSpeed,
			transform.position.y,
			transform.position.z +(float)Mathf.Sin(alpha)* moveSpeed);

		transform.position = Vector3.MoveTowards(start, Vector3.Lerp(start, end, deltaTime), moveSpeed);
	}
	void Rotating()
	{
		if (Input.GetKey(KeyCode.J))
			OnRotate(transform.eulerAngles.y-1);
		else if (Input.GetKey(KeyCode.K))
			OnRotate(transform.eulerAngles.y+1);
	}
	public void OnRotate(float beta)
	{

		// Normalize the target angle
		float normalizedAngle = NormalizeAngle(beta);

		// Calculate the difference between the target angle and the current angle
		float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, normalizedAngle);

		// If the difference is significant, rotate towards the target angle
		if (Mathf.Abs(deltaAngle) > 0.01f)
		{
			// Determine the direction of rotation
			float rotationDirection = Mathf.Sign(deltaAngle);

			// Rotate the player smoothly
			transform.Rotate(0, rotateSpeed * Time.deltaTime * rotationDirection, 0);
		}

	}
	void OnBeingHit(int damage)
	{
		int damageDeal = damage;
	}

    public void AttackNearbyObjects()
    {
            Debug.Log("Player ATTACK");
		if (myCombat.AttackCooldown > 0)
        {
            return; // Not ready to attack yet
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag(GameTags.Enemy);

        foreach (GameObject target in targets)
        {
			if (target != null)
			{
				float distance = Vector3.Distance(transform.position, target.transform.position);
				if (distance <= attackRange)
				{
                    BaseEnemy targetStats = target.GetComponent<BaseEnemy>();
					Debug.Log("HELOOOO " + targetStats);
					if (targetStats != null)
					{
						Debug.Log("HELOOOO khjkhkhjk" + target);
						myCombat.Attack(targetStats.MyStats);
                    }
				}
			}
        }
    }

    float NormalizeAngle(float angle)
	{
		while (angle < 0)
		{
			angle += 360;
		}
		while (angle >= 360)
		{
			angle -= 360;
		}
		return angle;
	}
}
