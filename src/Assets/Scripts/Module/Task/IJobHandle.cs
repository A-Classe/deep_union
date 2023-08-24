using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Task
{
    /// <summary>
    /// タスクにジョブを送信するインターフェース
    /// </summary>
    public interface IJobHandle
    {
        TaskState State { get; }
        void ExecuteJob();
    }
}