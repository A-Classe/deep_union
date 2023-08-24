using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input.Editor
{
    [CreateAssetMenu(menuName = "InputActionAssetProfile", fileName = "InputActionAssetProfile")]
    public class InputActionAssetProfile : ScriptableObject
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        public InputActionAsset GetUsingAsset()
        {
            return inputActionAsset;
        }
    }
}