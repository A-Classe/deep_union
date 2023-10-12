using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Utility
{
    /// <summary>
    /// 指定サイズ以下になると自動でリストに追加を行うList拡張
    /// </summary>
    /// <typeparam name="T">リストの型</typeparam>
    public class AutoInstanceList<T> : List<T> where T : MonoBehaviour
    {
        private GameObject instancePrefab;
        private Transform parentTransform;
        private uint buffer;

        private Vector2 randomSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefab">生成するインスタンス</param>
        /// <param name="parent">生成したobjを入れる親</param>
        /// <param name="buffer">自動生成のトリガーになるサイズ</param>
        public AutoInstanceList(GameObject prefab, Transform parent, uint buffer, Vector2 randomSize)
        {
            instancePrefab = prefab;
            parentTransform = parent;
            this.buffer = buffer;
            this.randomSize = randomSize;
        }

        public void SetList(List<T> list)
        {
            foreach (var monoBehaviour in list)
            {
                Add(monoBehaviour);
            }
        }

        private void CheckAndRefillBuffer()
        {
            if (Count <= buffer)
            {
                if (instancePrefab == null || parentTransform == null) return;
                int itemsToInstantiate = (int)buffer - Count;
                for (int i = 0; i < itemsToInstantiate; i++)
                {
                    AddToBufferAsync();
                }
            }
        }
        
        public new bool Remove(T item)
        {
            bool result = base.Remove(item);
            CheckAndRefillBuffer();
            return result;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            CheckAndRefillBuffer();
        }

        private void AddToBufferAsync()
        {
            CoroutineRunner.StartCoroutine(CreateInstance());
        }

        private IEnumerator CreateInstance()
        {
            yield return new WaitForEndOfFrame();
            // random area
            double theta = 2.0f * Math.PI * Random.value;
            double radius = randomSize.x * Math.Sqrt(Random.value);
            Vector3 spawn = new Vector3(
                parentTransform.position.x + (float)(radius * Math.Cos(theta)), 
                parentTransform.position.y,
                parentTransform.position.z + (float)(radius * Math.Sin(theta))
            );
            
            var instance = Object.Instantiate(instancePrefab, spawn, Quaternion.identity, parentTransform);
            instance.name = "Point_Clone";
            T component = instance.GetComponent<T>();
            if (component)
            {
                Add(component);
            }
        }
    }

    public static class CoroutineRunner
    {
        private static CoroutineRunnerBehaviour _coroutineRunner = null;
        private static CoroutineRunnerBehaviour coroutineRunner
        {
            get
            {
                if (_coroutineRunner == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    _coroutineRunner = go.AddComponent<CoroutineRunnerBehaviour>();
                }
                return _coroutineRunner;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return coroutineRunner.StartCoroutine(routine);
        }
    }

    public class CoroutineRunnerBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}