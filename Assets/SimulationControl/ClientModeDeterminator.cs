using System;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ClientModeDeterminator:ComponentSystem
{
	public static bool isClient{get;protected set;}
	public static bool isServer{get;protected set;}

	protected override void OnUpdate()
	{
		SetClientMode(!Application.isBatchMode);
		#if UNITY_EDITOR
			SetServerMode(true);
		#else
			SetServerMode(false);
		#endif
		Enabled=false;
	}

	public void SetClientMode(bool mode)
	{
		isClient=mode;
		World.GetExistingSystem<PresentationSystemGroup>().Enabled=mode;
	}
	public void SetServerMode(bool mode)
	{
		isServer=mode;
		if(mode)
		{
			World.GetExistingSystem<ClientNetworkingSystem>().Enabled=false;
			try
			{
				World.GetExistingSystem<ServerNetworkingSystem>().Enabled=true;
			}
			catch(Exception e)
			{
				Debug.Log("Failed to initiate server networking.");
				Debug.LogError(e);
			}
		}
		else
		{
			World.GetExistingSystem<ServerNetworkingSystem>().Enabled=false;
			try
			{
				World.GetExistingSystem<ClientNetworkingSystem>().Enabled=true;
			}
			catch(Exception e)
			{
				Debug.Log("Failed to initiate client networking.");
				Debug.LogError(e);
			}
		}
	}
}