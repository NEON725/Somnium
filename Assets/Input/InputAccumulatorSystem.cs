using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InputAccumulatorSystem:ComponentSystem
{
	protected double lastFrame;
	protected NativeList<FrameInputSnapshot> inputBuffer;
	protected PlayerInputBugfixInjector input=PlayerInputBugfixInjector.Get();

	protected override void OnStartRunning()
	{
		input.Enable();
		inputBuffer=new NativeList<FrameInputSnapshot>(10,Allocator.Persistent);
	}
	protected override void OnStopRunning()
	{
		input.Disable();
		inputBuffer.Dispose();
	}

	protected override void OnUpdate()
	{
		Cursor.lockState=CursorLockMode.Locked;

		double elapsedTime=UnityEngine.Time.time;
		double deltaTime=elapsedTime-lastFrame;
		FrameInputSnapshot snapshot=new FrameInputSnapshot()
		{
			elapsedTime=elapsedTime,
			inputs=new FrameInputComponent(input,deltaTime)
		};
		inputBuffer.Add(snapshot);
		lastFrame=elapsedTime;
	}

	public FrameInputComponent GetInputsForFrame(TimeData frameTime,bool remove=false)
	{
		double sinceElapsed=frameTime.ElapsedTime-frameTime.DeltaTime;
		double untilElapsed=frameTime.ElapsedTime;
		FrameInputComponent retVal=default(FrameInputComponent);
		for(int i=0;i<inputBuffer.Length;i++)
		{
			FrameInputSnapshot snapshot=inputBuffer[i];
			if(snapshot.elapsedTime>sinceElapsed&&snapshot.elapsedTime<=untilElapsed)
			{
				if(retVal.deltaTime==0){retVal=snapshot.inputs;}
				else{retVal.AppendFrom(snapshot.inputs);}
				if(remove)
				{
					inputBuffer.RemoveAtSwapBack(i);
					i--;
				}
			}
		}
		return retVal;
	}
}