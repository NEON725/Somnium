using Unity.Core;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FrameGovernor:ComponentSystem
{
	public const double FULL_FRAME_INTERVAL=0.03125;

	protected TakeSimulationSnapshotSystem snapshotSystem;
	protected double lastSubFrame;

	protected override void OnUpdate()
	{
		var simulationGroup=World.GetExistingSystem<SimulationSystemGroup>();
		var lateSimulationGroup=World.GetExistingSystem<LateSimulationSystemGroup>();

		snapshotSystem=World.GetOrCreateSystem<TakeSimulationSnapshotSystem>();
		lateSimulationGroup.AddSystemToUpdateList(snapshotSystem);
		simulationGroup.RemoveSystemFromUpdateList(snapshotSystem);

		simulationGroup.UpdateCallback=SimulationTickController;

		lastSubFrame=0;
		Enabled=false;
	}
	protected bool SimulationTickController(ComponentSystemBase simulationGroup)
	{
		double elapsed=UnityEngine.Time.time;
		double lastFullFrame=snapshotSystem.GetSnapshotElapsedTime();
		var sinceFullFrame=elapsed-lastFullFrame;
		if(sinceFullFrame>=FULL_FRAME_INTERVAL)
		{
			snapshotSystem.RestoreSnapshot();
			snapshotSystem.PrepareSnapshot();
			lastFullFrame+=FULL_FRAME_INTERVAL;
			World.PushTime(new TimeData(lastFullFrame,(float)FULL_FRAME_INTERVAL));
			lastSubFrame=lastFullFrame;
			UnityEngine.Time.fixedDeltaTime=(float)FULL_FRAME_INTERVAL;
			return true;
		}
		else if(lastSubFrame!=elapsed)
		{
			var deltaTime=elapsed-lastSubFrame;
			World.PushTime(new TimeData(elapsed,(float)deltaTime));
			lastSubFrame=elapsed;
			UnityEngine.Time.fixedDeltaTime=(float)deltaTime;
			return true;
		}
		return false;
	}
}
public class TakeSimulationSnapshotSystem:SystemBase
{
	protected SnapshotWorld snapshotWorld;
	protected bool snapshotPrepared=true;

	public TakeSimulationSnapshotSystem(){snapshotWorld=new SnapshotWorld();}

	protected void ReplaceWorld(World source,World destination)
	{
		var sourceTime=source.Time;
		var destTime=destination.Time;
		if(sourceTime.ElapsedTime!=destTime.ElapsedTime)
		{
			destination.EntityManager.CopyAndReplaceEntitiesFrom(source.EntityManager);
			destination.SetTime(source.Time);
		}
	}

	public void PrepareSnapshot(){snapshotPrepared=true;}
	public void RestoreSnapshot(){ReplaceWorld(snapshotWorld,World);}

	public double GetSnapshotElapsedTime(){return snapshotWorld.Time.ElapsedTime;}

	protected override void OnUpdate()
	{
		if(snapshotPrepared)
		{
			ReplaceWorld(World,snapshotWorld);
			snapshotPrepared=false;
		}
	}
}