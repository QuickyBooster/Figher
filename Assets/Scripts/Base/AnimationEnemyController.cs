using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnemyController : MonoBehaviour
{
	Animator animator;
	BaseEnemy baseEnemy;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		baseEnemy = GetComponent<BaseEnemy>();
	}
	void Update()
	{
	}
}
