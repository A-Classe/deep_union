using System;
using GameMain.Presenter;
using UnityEngine;
using VContainer;

namespace Module.Task
{
    /// <summary>
    /// リソースを管理するクラス
    /// </summary>
    public class ResourceContainer
    {
        private readonly GameParam gameParam;

        [Inject]
        public ResourceContainer(GameParam gameParam)
        {
            this.gameParam = gameParam;
        }

        /// <summary>
        /// リソース数
        /// </summary>
        public int ResourceCount
        {
            set { resourceCount = Mathf.Clamp(value, 0, gameParam.MaxResourceCount); }
            get { return resourceCount; }
        }
        
        /// <summary>
        /// リソースの最大数
        /// </summary>
        public int MaxResourceCount => gameParam.MaxResourceCount;

        /// <summary>
        /// リソース数が変化した時に送信されるイベント。(前のリソース数, 現在のリソース数)
        /// </summary>
        public event Action<int, int> OnResourceChanged;

        private int resourceCount;

        public void Add(int count)
        {
            int prevResourceCount = resourceCount;
            ResourceCount += count;

            OnResourceChanged?.Invoke(prevResourceCount, resourceCount);
        }

        public void Remove(int count)
        {
            int prevResourceCount = resourceCount;
            ResourceCount -= count;

            OnResourceChanged?.Invoke(prevResourceCount, resourceCount);
        }
    }
}