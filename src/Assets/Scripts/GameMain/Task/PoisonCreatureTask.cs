using System.Collections;
using System.Collections.Generic;
using Module.Task;
using UnityEngine;

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