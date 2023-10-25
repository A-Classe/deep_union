using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        public bool EnableDebugger = false;

        public float AssignIntensity = 1f;
        public float ReleaseIntensity = 0.3f;

        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public float MoveAccelaration = 1f;
        public float TorqueAccelaration = 1f;
        public float AngleLimit = 1f;
        public float MaxSpeed = 1f;

        public int HitPoint = 100;

        public float ActivateTaskRange = 1f;
        public float DeactivateTaskRange = 0f;
    }
}