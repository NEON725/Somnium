using System;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FrameGovernor:ComponentSystem
{
	public const double FULL_FRAME_INTERVAL=0.03125;
	public const double MAXIMUM_SUBFRAME_DEVIATION_PERCENT=0.25;

	protected TakeSimulationSnapshotSystem snapshotSystem;
	protected InputAccumulatorSystem inputAccumulatorSystem;
	protected double lastSubFrame=0;
	protected double lastReceivedServerFrame=0;
	protected double userVisibleWorldElapsed=0;
	protected double serverFrameLead=0;
	protected bool slavedToRemoteGovernor=false;
	protected bool createdInputSingleton=false;

	protected override void OnUpdate()
	{
		var simulationGroup=World.GetExistingSystem<SimulationSystemGroup>();
		simulationGroup.UpdateCallback=SimulationTickController;

		var initializationGroup=World.GetExistingSystem<InitializationSystemGroup>();
		var updateWorldTimeSystem = World.GetExistingSystem<UpdateWorldTimeSystem>();
		initializationGroup.RemoveSystemFromUpdateList(updateWorldTimeSystem);

		inputAccumulatorSystem=World.GetExistingSystem<InputAccumulatorSystem>();
		snapshotSystem=World.GetExistingSystem<TakeSimulationSnapshotSystem>();

		Enabled=false;
	}

	protected override void OnStartRunning()
	{
		if(!createdInputSingleton)
		{
			World.EntityManager.CreateEntity(ComponentType.ReadOnly<InputStateComponent>());
			createdInputSingleton=true;
		}
	}
	protected bool SimulationTickController(ComponentSystemBase simulationGroup)
	{
		double realTime=UnityEngine.Time.time;
		double worldTime=World.Time.ElapsedTime;
		double snapshotTime=snapshotSystem.GetSnapshotElapsedTime();
		double sinceLastSubFrame=realTime-lastSubFrame;

		if(!snapshotSystem.snapshotTaken){snapshotSystem.TakeSnapshot();}

		if(sinceLastSubFrame>0)
		{
			double advanceUserVisibleWorldElapsed=sinceLastSubFrame;
			if(slavedToRemoteGovernor)
			{
				double sinceLastReceivedServerFrame=realTime-lastReceivedServerFrame;
				double targetWorldElapsed=snapshotTime+sinceLastReceivedServerFrame+serverFrameLead;
				double minUserVisibleElapsed=worldTime+sinceLastSubFrame*(1-MAXIMUM_SUBFRAME_DEVIATION_PERCENT);
				double maxUserVisibleElapsed=worldTime+sinceLastSubFrame*(1+MAXIMUM_SUBFRAME_DEVIATION_PERCENT);
				advanceUserVisibleWorldElapsed=Math.Min(maxUserVisibleElapsed,Math.Max(minUserVisibleElapsed,targetWorldElapsed))-worldTime;
			}
			userVisibleWorldElapsed+=advanceUserVisibleWorldElapsed;
			inputAccumulatorSystem.BakeFrameInputs(new TimeData(userVisibleWorldElapsed,(float)advanceUserVisibleWorldElapsed));
			lastSubFrame=realTime;
		}

		double sinceLastFullFrame=userVisibleWorldElapsed-snapshotTime;
		if(!slavedToRemoteGovernor&&sinceLastFullFrame>=FULL_FRAME_INTERVAL)
		{
			snapshotSystem.RestoreSnapshot();
			snapshotSystem.PrepareSnapshot();
			return EngageFrame(FULL_FRAME_INTERVAL,true);
		}
		double thisSimulationStep=userVisibleWorldElapsed-worldTime;
		if(thisSimulationStep<=0){return false;}
		return EngageFrame(thisSimulationStep,false);
	}
	protected bool EngageFrame(double worldDelta,bool trimInputs)
	{
		TimeData frameTimeData=new TimeData(World.Time.ElapsedTime+worldDelta,(float)worldDelta);
		World.SetTime(frameTimeData);
		UnityEngine.Time.fixedDeltaTime = (float)worldDelta;
		InputStateComponent inputState = default;
		inputState = inputAccumulatorSystem.GetInputsForFrame(frameTimeData,trimInputs);
		SetSingleton(inputState);
		return true;
	}
}