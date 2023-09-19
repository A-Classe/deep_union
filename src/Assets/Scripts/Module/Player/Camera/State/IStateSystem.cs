namespace System
{
    public interface IStateSystem<out T>
    {
        public T GetState();

        public void Update();
    }
}