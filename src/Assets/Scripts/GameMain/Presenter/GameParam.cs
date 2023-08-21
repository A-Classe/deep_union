using UnityEngine;

namespace GameMain.Presenter
{
    [CreateAssetMenu(fileName = "GameParam")]
    public class GameParam : ScriptableObject
    {
        [Header("割り当て間隔時間")] [SerializeField] private float assignInterval = 0.5f;

        public float AssignInterval => assignInterval;
    }
}