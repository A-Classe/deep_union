using System.Linq;
using UnityEngine;

namespace Module.Task
{
    public static class TaskUtil
    {
        /// <summary>
        ///     シーン内の全てのタスクを取得します(安全なものに置き換え予定)
        /// </summary>
        /// <typeparam name="TInterface">キャストするクラス</typeparam>
        /// <returns></returns>
        public static TInterface[] FindSceneTasks<TInterface>() where TInterface : class
        {
            const string taskTag = "Task";
            var objects = GameObject.FindGameObjectsWithTag(taskTag);

            return objects
                .Select(obj =>
                {
                    if (obj.TryGetComponent(out TInterface task)) return task;

                    Debug.LogWarning($"GameObject: {obj.name}に{nameof(TInterface)}がアタッチされていません!");
                    return null;
                })
                .ToArray();
        }
    }
}