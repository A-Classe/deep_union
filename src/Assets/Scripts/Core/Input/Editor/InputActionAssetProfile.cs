using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputActionAssetProfile", fileName = "InputActionAssetProfile")]
public class InputActionAssetProfile : ScriptableObject
{
    [SerializeField] private InputActionAsset inputActionAsset;

    public InputActionAsset GetUsingAsset()
    {
        return inputActionAsset;
    }
}