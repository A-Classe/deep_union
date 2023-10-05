namespace Core.User
{
    public interface IDefaultable<T>
    {
        T DefaultInstance();
    }
}