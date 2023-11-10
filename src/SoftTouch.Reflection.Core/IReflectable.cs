namespace SoftTouch.Reflection.Core;


public interface IReflectable
{
    public T Get<T>(string name);
    public void Set<T>(string name, T value);
}