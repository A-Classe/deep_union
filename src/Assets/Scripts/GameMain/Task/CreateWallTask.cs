using System.Collections;
using System.Collections.Generic;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class CreateWallTask : BaseTask
    {
        [SerializeField] private GameObject body;
        private RuntimeNavMeshBaker navMeshBaker;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
        }

        protected override void OnComplete()
        {
            body.SetActive(true);
            navMeshBaker.Bake().Forget();
        }
    }
}