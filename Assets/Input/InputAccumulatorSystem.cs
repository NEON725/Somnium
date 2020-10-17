using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ClientModeDeterminator))]
public class InputAccumulatorSystem:ComponentSystem
{
	protected double lastFrame;
	protected NativeList<FrameInputSnapshot> inputBuffer;
	protected InputStateComponent lastFrameState;
	protected PlayerInputBugfixInjector input=PlayerInputBugfixInjector.Get();

	protected override void OnStartRunning()
	{
		input.Enable();
		inputBuffer=new NativeList<FrameInputSnapshot>(100,Allocator.Persistent);
	}
	protected override void OnStopRunning()
	{
		input.Disable();
		inputBuffer.Dispose();
	}

	protected override void OnUpdate()
	{
		if(ClientModeDeterminator.isClient)
		{
			Cursor.lockState=CursorLockMode.Locked;
			lastFrameState=new InputStateComponent(input);
		}
	}

	public InputStateComponent BakeFrameInputs(TimeData time)
	{
		InputStateComponent retVal=lastFrameState;
		inputBuffer.Add(new FrameInputSnapshot(retVal,time));
		lastFrameState=default;
		return retVal;
	}

	public InputStateComponent GetInputsForFrame(TimeData frameTime,bool removeSnapshotParam=false)
	{
		double untilElapsed=frameTime.ElapsedTime;
		double deltaElapsed=frameTime.DeltaTime;
		double sinceElapsed=untilElapsed-deltaElapsed;
		InputStateComponent retVal=default;
		for(int i=0;i<inputBuffer.Length;i++)
		{
			FrameInputSnapshot snapshot=inputBuffer[i];
			InputStateComponent snapshotState=snapshot.state;
			double snapshotEnd=snapshot.time.ElapsedTime;
			double snapshotDelta=snapshot.time.DeltaTime;
			double snapshotBegin=snapshotEnd-snapshotDelta;
			double portion=0;
			bool removeSnapshot=false;
			if(snapshotBegin>sinceElapsed&&snapshotEnd<=untilElapsed)
			{
				portion=1;
				removeSnapshot=true;
			}
			else if(snapshotBegin>sinceElapsed&&snapshotBegin<untilElapsed)
			{
				portion=(untilElapsed-snapshotBegin)/snapshotDelta;
			}
			else if(snapshotEnd>sinceElapsed&&snapshotEnd<untilElapsed)
			{
				portion=(snapshotEnd-sinceElapsed)/snapshotDelta;
				removeSnapshot=true;
			}
			else if(snapshotBegin<sinceElapsed&&snapshotEnd>untilElapsed)
			{
				portion=deltaElapsed/snapshotDelta;
				removeSnapshot=true;
			}
			if(portion>0)
			{
				snapshotState.ApplyScale(portion);
				retVal+=snapshotState;
			}
			if(removeSnapshotParam&&removeSnapshot)
			{
				inputBuffer.RemoveAtSwapBack(i);
				i--;
			}
		}
		return retVal;
	}
}