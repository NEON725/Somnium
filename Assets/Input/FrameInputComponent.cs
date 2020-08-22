using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct FrameInputComponent:IComponentData
{
	public double deltaTime;
	public float2 look;
	public float2 move;
	public bool jump;

	public FrameInputComponent(PlayerInputBugfixInjector input,double deltaTime)
	{
		this.deltaTime=deltaTime;
		AssignWithScale(out look,input.GetLookFixed(),deltaTime);
		AssignWithScale(out move,input.Player.Move.ReadValue<Vector2>(),deltaTime);
		jump=input.Player.Jump.triggered;
	}

	public void AppendFrom(FrameInputComponent other)
	{
		AppendFrom(ref look,other.look);
		AppendFrom(ref move,other.move);
		jump|=other.jump;
		deltaTime+=other.deltaTime;
	}

	public static void AppendFrom(ref float2 a,in float2 b)
	{
		a.x+=b.x;
		a.y+=b.y;
	}
	public static void AssignWithScale(out float2 a,in float2 b,double scale)
	{
		a.x=(float)(b.x*scale);
		a.y=(float)(b.y*scale);
	}
}

public struct FrameInputSnapshot
{
	public double elapsedTime;
	public FrameInputComponent inputs;
}