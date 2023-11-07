using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Task;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class CreateWallTask : BaseTask
    {
        [SerializeField] private GameObject[] types;
        private RuntimeNavMeshBaker navMeshBaker;

        private int currentIndex;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();

            OnProgressChanged += OnProgress;
            currentIndex = 0;
            UpdateObject();
        }

        /// <summary>
        /// progressのcallbackを受け取る
        /// typesのlengthとcurrentIndexから次に切り替えが必要なタイミングを見つける
        /// </summary>
        /// <param name="value">progress count 0..1</param>
        private void OnProgress(float value)
        {
            float range = (float)(currentIndex + 1) / (types.Length - 1);
            if (value >= range)
            {
                currentIndex++;
                UpdateObject();
            }
        }

        protected override void OnComplete()
        {
            navMeshBaker.Bake().Forget();
        }

        /// <summary>
        /// currentIndexに応じてobjectを切り替える
        /// </summary>
        private void UpdateObject()
        {
            foreach (var type in types)
            {
                type.SetActive(false);
            }

            types[currentIndex].SetActive(true);
        }
    }
}