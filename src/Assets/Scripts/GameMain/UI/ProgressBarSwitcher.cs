using System;
using System.Collections.Generic;
using System.Linq;
using Module.Task;
using UI.HUD;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace GameMain.UI
{
    public class ProgressBarSwitcher : IStartable, ITickable
    {
        private readonly Camera mainCamera;
        private readonly TaskProgressPool taskProgressPool;
        private readonly Transform[] taskTransforms;
        private readonly Queue<TaskProgressView> activeViews;

        private int head;
        private int tail;

        [Inject]
        public ProgressBarSwitcher(TaskProgressPool taskProgressPool)
        {
            mainCamera = Camera.main;
            activeViews = new Queue<TaskProgressView>();
            this.taskProgressPool = taskProgressPool;
            taskTransforms = SortTaskOrder(TaskUtil.FindSceneTasks<Transform>());
        }

        private Transform[] SortTaskOrder(IEnumerable<Transform> transforms)
        {
            float camZ = mainCamera.transform.position.z;
            return transforms.OrderBy(transform => transform.position.z - camZ).ToArray();
        }

        public void Start()
        {
            foreach (Transform transform in taskTransforms)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

                if (IsPassed(viewPos))
                {
                    head++;
                    continue;
                }

                if (!IsAhead(viewPos))
                {
                    TaskProgressView view = taskProgressPool.AddProgressView(transform);
                    activeViews.Enqueue(view);
                    tail++;
                }
            }
        }

        public void Tick()
        {
            UpdateHead();
            UpdateTail();

            foreach (TaskProgressView progressView in activeViews)
            {
                progressView.ManagedUpdate();
            }
        }

        void UpdateHead()
        {
            if (head >= taskTransforms.Length)
                return;

            Transform headTransform = taskTransforms[head];
            Vector3 viewPos = mainCamera.WorldToViewportPoint(headTransform.position);

            if (IsPassed(viewPos))
            {
                TaskProgressView view = activeViews.Dequeue();
                taskProgressPool.RemoveProgressView(view);
                head++;
            }
        }

        void UpdateTail()
        {
            if (tail + 1 >= taskTransforms.Length)
                return;

            Transform tailTransform = taskTransforms[tail + 1];
            Vector3 viewPos = mainCamera.WorldToViewportPoint(tailTransform.position);

            if (!IsAhead(viewPos))
            {
                TaskProgressView view = taskProgressPool.AddProgressView(tailTransform);
                activeViews.Enqueue(view);
                tail++;
            }
        }


        bool IsAhead(Vector4 viewPos)
        {
            return viewPos.y > 1;
        }

        bool IsPassed(Vector3 viewPos)
        {
            return viewPos.y < 0;
        }
    }
}