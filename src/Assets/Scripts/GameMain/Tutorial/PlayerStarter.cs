using System;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using Module.Player.State;
using UnityEngine;

namespace GameMain.Tutorial
{
    public class PlayerStarter : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private string Tag = "Player";

        private async void Start()
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(100));

            playerController.SetState(PlayerState.Stop);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag))
            {
                playerController.SetState(PlayerState.Auto);
            }
        }
    }
}