using Unity.Entities;
using Unity.Mathematics;

public struct LocalPlayer:IComponentData
{
	public float2 accumulatedMouseLook;
}