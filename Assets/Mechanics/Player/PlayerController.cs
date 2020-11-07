using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class LocalPlayerController:ComponentSystem
{
	public const float LOOK_SPEED=0.5f;
	public const float MOVE_SPEED=16f;
	public const float JUMP_FORCE=8;
	public const float CAMERA_FOLLOW_DISTANCE=8;

	protected static float3 WORLD_FORWARD=new float3(0,0,1);
	protected static float3 WORLD_BACKWARD=new float3(0,0,-1);
	protected static float3 WORLD_UP=new float3(0,1,0);
	protected Camera followCam;

	protected override void OnStartRunning()
	{
		followCam=Camera.main;
		//Rotation is locked by setting inertia to infinite.
		Entities
			.WithAll<LocalPlayer>()
			.ForEach((ref PhysicsMass phys)=>
			{
				phys.InverseInertia=float3.zero;
			});
	}

	protected override void OnUpdate()
	{
		float3 WORLD_FORWARD=LocalPlayerController.WORLD_FORWARD;
		float3 WORLD_BACKWARD=LocalPlayerController.WORLD_BACKWARD;
		float3 WORLD_UP=LocalPlayerController.WORLD_UP;

		Entities.ForEach((ref PhysicsVelocity physics,ref LocalPlayer player,ref Translation translation,ref Rotation rotation)=>
		{
			InputStateComponent input=GetSingleton<InputStateComponent>();
			float2 controlLook=input.look*LOOK_SPEED;
			/*
			* Note the confusing X/Y usage, because yaw rotates
			*  AROUND the Y axis, based on cursor movement ALONG
			*  the X axis.
			*/
			quaternion applyYaw=quaternion.RotateY(controlLook.x);
			float3 forward=math.mul(math.mul(rotation.Value,applyYaw),WORLD_FORWARD);
			forward.y=0;
			forward=math.normalize(forward);
			rotation.Value=quaternion.LookRotation(forward,WORLD_UP);

			float3 right=math.mul(quaternion.RotateY(math.PI/2),forward);
			float3 applyMove=forward*input.move.y+right*input.move.x;
			translation.Value+=applyMove*MOVE_SPEED;

			if(input.jump){physics.Linear.y+=JUMP_FORCE;}

			player.followCameraPitch=math.clamp(player.followCameraPitch+controlLook.y,math.PI/-2,math.PI/2);
			quaternion cameraPitch=quaternion.RotateX(-player.followCameraPitch);
			quaternion completeCameraRotation=math.mul(rotation.Value,cameraPitch);
			float3 followOffset=math.mul(completeCameraRotation,WORLD_BACKWARD)*CAMERA_FOLLOW_DISTANCE;
			followCam.transform.rotation=completeCameraRotation;
			followCam.transform.position=translation.Value+followOffset;
		});
	}
}