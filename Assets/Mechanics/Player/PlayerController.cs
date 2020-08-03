using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class MouseLookAccumulatorSystem:JobComponentSystem
{
	private PlayerInputBugfixInjector input=PlayerInputBugfixInjector.Get();

	protected override JobHandle OnUpdate(JobHandle prevJobs)
	{
		Entities
			.WithoutBurst()
			.ForEach((ref LocalPlayer localPlayer)=>
			{
				bool freeMouse=input.Player.FreeMouse.triggered;
				Cursor.lockState=freeMouse?CursorLockMode.None:CursorLockMode.Locked;

				if(!freeMouse){localPlayer.accumulatedMouseLook+=(float2)input.GetLookFixed();}
			}).Run();
		return prevJobs;
	}
}

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class LocalPlayerController:JobComponentSystem
{
	public float lookSpeed=0.02f;
	public float moveSpeed=0.25f;
	public float jumpForce=8;
	public float cameraFollowDistance=8;

	private float3 worldForward=new float3(0,0,1);
	private float3 worldBackward=new float3(0,0,-1);
	private float3 worldUp=new float3(0,1,0);
	private PlayerInputBugfixInjector input=PlayerInputBugfixInjector.Get();

	private float followCameraPitch=0;

	protected override void OnStartRunning()
	{
		input.Enable();
		/*
		* The "Freeze Constraints" property does not survive entity authoring.
		* Instead, the effect is replicated by giving it infin
		*/
		Entities
			.WithoutBurst()
			.WithAll<LocalPlayer>()
			.ForEach((ref PhysicsMass phys)=>
			{
				phys.InverseInertia=float3.zero;
			}).Run();
	}

	protected override JobHandle OnUpdate(JobHandle prevJobs)
	{
		Transform cameraTransform=Camera.main.gameObject.GetComponent<Transform>();
		float2 controlMove=input.Player.Move.ReadValue<Vector2>();
		bool controlJump=input.Player.Jump.triggered;

		Entities
			.WithoutBurst()
			.ForEach((ref PhysicsVelocity physics,ref Translation translation,ref Rotation rotation,ref LocalPlayer localPlayer)=>
		{
			float2 controlLook=localPlayer.accumulatedMouseLook*lookSpeed;
			localPlayer.accumulatedMouseLook=float2.zero;
			/*
			* Note the confusing X/Y usage, because yaw rotates
			*  AROUND the Y axis, based on cursor movement ALONG
			*  the X axis.
			*/
			quaternion applyYaw=quaternion.RotateY(controlLook.x);
			float3 forward=math.mul(math.mul(rotation.Value,applyYaw),worldForward);
			forward.y=0;
			forward=math.normalize(forward);
			rotation.Value=quaternion.LookRotation(forward,worldUp);
			float3 right=math.mul(quaternion.RotateY(math.PI/2),forward);
			float3 applyMove=forward*controlMove.y+right*controlMove.x;
			translation.Value+=applyMove*moveSpeed;

			if(controlJump){physics.Linear.y+=jumpForce;}

			followCameraPitch=math.clamp(followCameraPitch+controlLook.y,math.PI/-2,math.PI/2);
			quaternion cameraPitch=quaternion.RotateX(-followCameraPitch);
			quaternion completeCameraRotation=math.mul(rotation.Value,cameraPitch);
			cameraTransform.rotation=completeCameraRotation;
			float3 followOffset=math.mul(completeCameraRotation,worldBackward)*cameraFollowDistance;
			cameraTransform.position=translation.Value+followOffset;
		}).Run();
		return prevJobs;
	}
}