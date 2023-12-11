namespace Core.Model.User
{
    public interface IDefaultable<T>
    {
        T DefaultInstance();
    }
}