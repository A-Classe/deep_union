using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module.Working
{
    [Serializable]
    public class RandomValueFloat
    {
        [SerializeField] private float min;
        [SerializeField] private float max;

        public float MakeValue()
        {
            return Random.Range(min, max);
        }
    }
}