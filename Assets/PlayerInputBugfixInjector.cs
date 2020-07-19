using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputBugfixInjector:DefaultPlayerInputs
{
	public Vector2 GetLookFixed()
	{
		return this.Player.Look.ReadValue<Vector2>()+new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
	}
}