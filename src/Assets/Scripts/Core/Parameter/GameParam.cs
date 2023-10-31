using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        public bool EnableDebugger = false;
        [Header("移動加速度")] public float MoveAccelaration = 1f;
        [Header("速度制限")] public float MinSpeed = 1f;
        public float MaxSpeed = 1f;
        [Header("回転加速度")] public float TorqueAccelaration = 1f;
        [Header("回転制限")] public float AngleLimit = 1f;

        [Space]
        public float AssignIntensity = 1f;
        public float ReleaseIntensity = 0.3f;

        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public int HitPoint = 100;

        public float ActivateTaskRange = 1f;
        public float DeactivateTaskRange = 0f;
    }
}