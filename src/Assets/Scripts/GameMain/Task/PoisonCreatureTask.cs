namespace Module.Task
{
    public class PoisonCreatureTask : BaseTask
    {
        protected override void OnComplete()
        {
            Disable();
        }
    }
}