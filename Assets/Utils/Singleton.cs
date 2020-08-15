using System.Runtime.CompilerServices;

public class Singleton<T> where T:new()
{
	private T instance;

	[MethodImpl(MethodImplOptions.Synchronized)]
	public T Get()
	{
		if(instance==null){instance=new T();}
		return instance;
	}
}