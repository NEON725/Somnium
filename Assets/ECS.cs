using System;
using System.Collections.Generic;

namespace ECS
{
	public class EntityRoster
	{
		private static List<Entity> roster=new List<Entity>();
		public static void RegisterEntity(Entity entity)=>roster.Add(entity);
		public static void UnregisterEntity(Entity entity)=>roster.Remove(entity);
		public static List<Entity> GetEntitiesByComponentTuple(List<Type> requiredComponentTypes)
		{
			List<Entity> retVal=new List<Entity>();
			foreach(Entity entity in roster)
			{
				bool matchesTuple=true;
				foreach(Type requiredType in requiredComponentTypes)
				{
					bool foundMatch=false;
					foreach(Component component in entity.components)
					{
						if(requiredType.IsAssignableFrom(component.GetType()))
						{
							foundMatch=true;
							break;
						}
					}
					if(!foundMatch)
					{
						matchesTuple=false;
						break;
					}
				}
				if(matchesTuple){retVal.Add(entity);}
			}
			return retVal;
		}
	}

	public class SystemRoster
	{
		private static List<System> roster=new List<System>();
		public static void RegisterSystem<T>() where T:System,new()
		{
			T system=new T();
			roster.Add(system);
		}
		public static void ExecuteSystems()
		{
			foreach(System system in roster){system.ExecuteAll();}
		}
	}

	public class Component
	{
	}

	public class System
	{
		protected List<Type> dependentComponentTypes=new List<Type>();
		protected void RegisterDependentComponentType<T>() where T:Component=>dependentComponentTypes.Add(typeof(T));

		public virtual void ExecuteAll()
		{
			List<Entity> applicableEntities=EntityRoster.GetEntitiesByComponentTuple(dependentComponentTypes);
			foreach(Entity entity in applicableEntities){ExecuteAgainst(entity);}
		}
		public virtual void ExecuteAgainst(Entity entity)=>throw new NotImplementedException();
	}
	public class SecondOrderSystem:System
	{
		public override void ExecuteAll()
		{
			List<Entity> applicableEntities=EntityRoster.GetEntitiesByComponentTuple(dependentComponentTypes);
			foreach(Entity a in applicableEntities)
			{
				foreach(Entity b in applicableEntities)
				{
					if(a!=b){ExecuteAgainst(a,b);}
				}
			}
		}
		public virtual void ExecuteAgainst(Entity a,Entity b)=>throw new NotImplementedException();
	}

	public class Entity:IDisposable
	{
		public Entity()=>EntityRoster.RegisterEntity(this);

		public List<Component> components {get;}
		protected void RegisterComponent<T>() where T:Component,new()
		{
			T component=new T();
			components.Add(component);
		}
		public T GetComponent<T>() where T:Component
		{
			foreach(Component comp in components)
			{
				T retVal=comp as T;
				if(retVal!=null){return retVal;}
			}
			return null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposeManaged)
		{
			if(disposeManaged){EntityRoster.UnregisterEntity(this);}
		}
		~Entity(){Dispose(false);}
	}
}