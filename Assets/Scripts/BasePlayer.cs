using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{

	float rotateSpeed { get; set; }
	float moveSpeed { get; set; }
	float lineOfSight { get; set; }
	int hitPoint { get; set; }
	Weapon weapon { get; set; }
	public BasePlayer(float rotate, float move, float sight, int hp)
	{
		this.rotateSpeed = rotate;
		this.moveSpeed = move;
		this.lineOfSight = sight;
		this.hitPoint = hp;
	}
	void Start()
	{
		rotateSpeed = 90;
		moveSpeed = 4;
		lineOfSight = 2;
		hitPoint = 100;

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
			if (Input.GetAxisRaw("Vertical") == 0)
			{ } else if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(Math.PI/2, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-Math.PI/2, Time.deltaTime);
		} else if (Input.GetAxisRaw("Horizontal") > 0)
		{
			if (Input.GetAxisRaw("Vertical") == 0)
				OnMoving(0, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(Math.PI/4, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-Math.PI/4, Time.deltaTime);
		} else if (Input.GetAxisRaw("Horizontal") < 0)
		{
			if (Input.GetAxisRaw("Vertical") == 0)
				OnMoving(Math.PI, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") > 0)
				OnMoving(3*Math.PI/4, Time.deltaTime);
			else if (Input.GetAxisRaw("Vertical") <0)
				OnMoving(-3*Math.PI/4, Time.deltaTime);
		}
	}
	void OnMoving(double alpha, float deltaTime)
	{
		Vector2 start = transform.position;
		Vector2 end = new Vector2(transform.position.x + (float)Math.Cos(alpha)* moveSpeed,
			transform.position.y +(float)Math.Sin(alpha)* moveSpeed);

		transform.position = Vector2.MoveTowards(start, Vector2.Lerp(start, end, deltaTime), moveSpeed);
	}
	void Rotating()
	{
		if (Input.GetKeyDown(KeyCode.J))
			OnRotate(transform.localEulerAngles.z-1);
		else if (Input.GetKeyDown(KeyCode.K))
			OnRotate(transform.localEulerAngles.z+1);
	}
	void OnRotate(float beta)
	{
		float delta = beta - transform.localEulerAngles.z;
		if (transform.localEulerAngles.z != beta)
			if (delta > 0)
				transform.Rotate(0, 0, rotateSpeed);
			else
				transform.Rotate(0, 0, -rotateSpeed);

	}
	void OnBeingHit(int damage, int critChance)
	{
		int damageDeal = damage;
		var x = UnityEngine.Random.Range(0, 100);
		if (x<critChance)
		{
			damageDeal = damage*2;
		}
		hitPoint -= damageDeal;
		if (hitPoint < 0)
		{
			hitPoint = 0;
		}
	}

}
