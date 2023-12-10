using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        [Header("１秒でHPが減るスピード")] public float DecereseHpSpeed = 1f;
        [Header("1回でHPが減る量")] public uint DecereseHpAmount = 1;

        [Space] public float AssignIntensity = 1f;
        public float ReleaseIntensity = 0.3f;

        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public int HitPoint = 100;

        public float ActivateTaskRange = 1f;
    }
}