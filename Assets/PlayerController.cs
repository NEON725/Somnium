using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController:MonoBehaviour
{
	public float moveSpeed;
	public float rotateSpeed;
	public float jumpForce;

	private PlayerInputBugfixInjector inputs;
	private Rigidbody body;
	private Vector3 moveVelocity;
	private Quaternion applyRot;
	private Camera mainCamera;

	public void Awake()
	{
		body=GetComponent<Rigidbody>();
		mainCamera=Camera.main;
		inputs=new PlayerInputBugfixInjector();
		inputs.Player.Jump.performed+=OnJump;
		Cursor.lockState=CursorLockMode.Locked;
	}
	public void OnEnable()=>inputs.Enable();
	public void OnDisable()=>inputs.Disable();

	public void Update()
	{
		Vector2 controlV=inputs.Player.Move.ReadValue<Vector2>();
		moveVelocity+=(transform.forward*controlV.y+transform.right*controlV.x)*moveSpeed;
		Vector2 controlR=inputs.GetLookFixed();
		applyRot=Quaternion.AngleAxis(controlR.x*rotateSpeed,Vector3.up);
	}

	public void OnJump(InputAction.CallbackContext unused)=>moveVelocity.y+=jumpForce;

	public void FixedUpdate()
	{
		moveVelocity.y+=body.velocity.y;
		body.velocity=moveVelocity;
		body.rotation=(body.rotation*applyRot).normalized;
		moveVelocity=Vector3.zero;
		applyRot=Quaternion.identity;
	}
}