using Module.Task;

namespace Module.Extension.Task
{
    public class PoisonCreatureTask : BaseTask
    {
        protected override void OnComplete()
        {
            Disable();
        }
    }
}