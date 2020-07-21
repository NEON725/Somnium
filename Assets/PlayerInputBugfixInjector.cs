using UnityEngine;
using Unity.Mathematics;

public class PlayerInputBugfixInjector:DefaultPlayerInputs
{
	private static Singleton<PlayerInputBugfixInjector> instance=new Singleton<PlayerInputBugfixInjector>();
	public static PlayerInputBugfixInjector Get(){return instance.Get();}

	public Vector2 GetLookFixed()
	{
		return this.Player.Look.ReadValue<Vector2>()+new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
	}
}