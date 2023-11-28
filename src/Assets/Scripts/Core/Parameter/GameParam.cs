using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        [Header("移動加速度")] public float MoveAccelaration = 1f;
        [Header("速度制限")] public float MinSpeed = 1f;
        public float MaxSpeed = 1f;


        [Header("回転速度制限")] public float MinRotateSpeed = 1f;
        public float MaxRotateSpeed = 1f;

        [Header("回転加速度")] public float TorqueAccelaration = 1f;
        [Header("回転制限")] public float AngleLimit = 1f;
        [Header("１秒でHPが減るスピード")] public float DecereseHpSpeed = 1f;
        [Header("1回でHPが減る量")] public uint DecereseHpAmount = 1;

        [Space] public float AssignIntensity = 1f;
        public float ReleaseIntensity = 0.3f;

        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public int HitPoint = 100;

        public float ActivateTaskRange = 1f;
        public float DeactivateTaskRange;
    }
}