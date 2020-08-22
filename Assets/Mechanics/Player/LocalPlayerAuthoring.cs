using Unity.Entities;
using UnityEngine;
public class LocalPlayerAuthoring:MonoBehaviour, IConvertGameObjectToEntity
{
	public void Convert(Entity ent, EntityManager manager, GameObjectConversionSystem conversion)
	{
		manager.AddComponent(ent,typeof(LocalPlayer));
		manager.AddComponent(ent,typeof(FrameInputComponent));
	}
}