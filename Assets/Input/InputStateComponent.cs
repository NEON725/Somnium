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
		InputStateComponent retVal=default;
		retVal.look=a.look+b.look;
		retVal.move=a.move+b.move;
		retVal.jump=a.jump|b.jump;
		return retVal;
	}
}