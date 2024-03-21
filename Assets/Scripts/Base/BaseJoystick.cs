using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

abstract public class BaseJoystick : MonoBehaviour
{
	[SerializeField]
	protected BasePlayer player;
	[SerializeField]
	protected GameObject background, joystick;
	[SerializeField]
	protected bool isTouching;
	public bool IsTouching
	{
		get { return isTouching; }
		set
		{
			isTouching = value;
			background.SetActive(value);
			joystick.SetActive(value);
			joystick.transform.localPosition = Vector3.zero;
		}
	}
	protected Vector3 controllerPosition;
	protected BoxCollider2D effectArea;
	private void Start()
	{
		background.SetActive(false);
		joystick.SetActive(false);
		effectArea = GetComponent<BoxCollider2D>();
		player = FindObjectOfType<BasePlayer>();
	}

	protected void Update()
	{
		if (player == null) return;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (effectArea.bounds.Contains(mousePosition))
            {
                OnTouch(Input.mousePosition);
            }
        }
        if (isTouching)
        {
            if (Input.GetMouseButton(0))
            {
                OnDrag(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnEndTouch();
        }
#endif

#if !UNITY_EDITOR
		if (Input.touchCount == 1)
		{
			if (!isTouching)
			{
				OnTouch(Input.GetTouch(0).position);
			} else
			{
				OnDrag(Input.GetTouch(0).position);
			}
		} else
		{
			OnEndTouch();
		}
#endif
	}

	virtual protected void OnTouch(Vector3 mousePos)
	{
		if (!isTouching)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
			pos.z = 0;
			controllerPosition = pos;
			IsTouching = true;
			background.transform.position = controllerPosition;
		}
	}

	abstract protected void OnDrag(Vector3 mousePos);
	virtual protected void OnEndTouch()
	{
		if (IsTouching)
		{
			if (player != null)
			{
			}
			IsTouching = false;
		}
	}

	// SINGLETON
	private static BaseJoystick _instance;
	public static BaseJoystick Instance
	{
		get
		{
			if (_instance == null)
			{
				//_instance = GameObject.FindObjectOfType<BaseJoystick>();
			}
			return _instance;
		}
	}
	private void Awake()
	{
		_instance = this;
	}
	private void OnEnable()
	{
		IsTouching = false;
	}
}
