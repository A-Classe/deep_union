namespace Core.Utility.User
{
    public interface IDefaultable<T>
    {
        T DefaultInstance();
    }
}