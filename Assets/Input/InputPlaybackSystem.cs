using Unity.Core;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class InputPlaybackSystem:ComponentSystem
{
	InputAccumulatorSystem accumulator;

	protected override void OnStartRunning()
	{
		accumulator=World.GetExistingSystem<InputAccumulatorSystem>();
	}

	protected override void OnUpdate()
	{
		TimeData frameTime=World.Time;
		double lastFrameTime=frameTime.ElapsedTime-frameTime.DeltaTime;
		FrameInputComponent aggregatedInput=accumulator.GetInputsForFrame(frameTime,frameTime.DeltaTime>=FrameGovernor.FULL_FRAME_INTERVAL);
		Entities.ForEach((ref FrameInputComponent input)=>
		{
			input=aggregatedInput;
		});
	}
}