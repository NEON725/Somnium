using Unity.Core;

public struct FrameInputSnapshot
{
	public InputStateComponent state;
	public TimeData time;

	public FrameInputSnapshot(InputStateComponent prevState,TimeData time)
	{
		this.state = prevState;
		this.time = time;
		state.ApplyScale(time.DeltaTime);
	}
}