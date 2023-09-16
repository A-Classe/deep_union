using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        public float AssignIntensity = 1f;
        public float ReleaseIntensity = 0.3f;
        
        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public float PlayerSpeed = 1f;

        public int HitPoint = 100;

        public float ActivateTaskRange = 1f;
        public float DeactivateTaskRange = 0f;
    }
}