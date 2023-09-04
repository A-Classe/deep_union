using UnityEngine;

namespace GameMain.Presenter
{
    public class GameParam : ScriptableObject
    {
        public float AssignInterval = 0.1f;
        public float ReleaseInteval = 0.1f;

        public float CollectFactor = 1f;
        public int TemoraryStrageCount = 10;
        public int MaxResourceCount = 999;

        public float PlayerSpeed = 1f;
    }
}