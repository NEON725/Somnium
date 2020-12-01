using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct InputStateComponent:IComponentData
{
	public float2 look;
	public float2 move;
	public bool jump;

	public InputStateComponent(PlayerInputBugfixInjector input)
	{
		look=input.GetLookFixed();
		move=input.Player.Move.ReadValue<Vector2>();
		jump=input.Player.Jump.triggered;
	}

	public InputStateComponent(float2 look,float2 move,bool jump)
	{
		this.look = look;
		this.move = move;
		this.jump = jump;
	}

	public void ApplyScale(double scale)
	{
		ApplyScale(ref look,scale);
		ApplyScale(ref move,scale);
	}

	private static void ApplyScale(ref float2 a,double scale)
	{
		a.x*=(float)scale;
		a.y*=(float)scale;
	}

	public static InputStateComponent operator+(InputStateComponent a,InputStateComponent b)
	{
		return new InputStateComponent(a.look+b.look,a.move+b.move,a.jump|b.jump);
	}
}