using System;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerMoverment : MonoBehaviour
{
	[SerializeField]
	BasePlayer player;

	[SerializeField]
	Vector2 moveVector;

	[SerializeField]
	Vector2 rotateVector;

	public void InputMovement(InputAction.CallbackContext context)
	{
		moveVector = context.ReadValue<Vector2>().normalized;
	}
	public void InputRotation(InputAction.CallbackContext context)
	{
		rotateVector = context.ReadValue<Vector2>().normalized;
	}

	private void Update()
	{
		Debug.Log("29");
		Debug.Log(moveVector.x);
		Debug.Log(player.ToString());
		if (moveVector != Vector2.zero && player != null)
		{
			Debug.Log(Mathf.Atan2(moveVector.y, moveVector.x));
			player.OnMoving(Mathf.Atan2(moveVector.y, moveVector.x), Time.deltaTime);
		}

		if (rotateVector != Vector2.zero && player != null)
		{
			// Calculate the angle of rotation
			float angle = Mathf.Atan2(rotateVector.y, rotateVector.x) * Mathf.Rad2Deg - 90f;
			// Pass the angle to the OnRotate method
			player.OnRotate(angle);
		}
	}
}
