using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class TakeSimulationSnapshotSystem:SystemBase
{
	protected SnapshotWorld snapshotWorld;
	protected bool snapshotPrepared=false;
	public bool snapshotTaken{get; protected set;}

	public TakeSimulationSnapshotSystem(){snapshotWorld=new SnapshotWorld();}

	protected static void ReplaceWorld(World source,World destination)
	{
		destination.EntityManager.CopyAndReplaceEntitiesFrom(source.EntityManager);
		destination.SetTime(source.Time);
	}

	public void PrepareSnapshot(){snapshotPrepared=true;}
	public void TakeSnapshot()
	{
		ReplaceWorld(World,snapshotWorld);
		snapshotPrepared=false;
		snapshotTaken=true;
	}
	public void RestoreSnapshot(){ReplaceWorld(snapshotWorld,World);}

	public double GetSnapshotElapsedTime(){return snapshotWorld.Time.ElapsedTime;}

	protected override void OnUpdate()
	{
		if(snapshotPrepared)
		{
			TakeSnapshot();
		}
	}
}