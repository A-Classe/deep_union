using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Task
{
    /// <summary>
    /// タスクにジョブを送信するインターフェース
    /// </summary>
    public interface IJobSender
    {
        TaskState State { get; }
        void Send();
    }
}