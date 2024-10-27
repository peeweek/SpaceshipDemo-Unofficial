using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX.DebugTools;

[RequireComponent(typeof(VFXDebugRuntimeView))]
public class VFXDebugSpaceshipMgt : MonoBehaviour
{
    [Header("Toggle Debug View (Input)")]
    public InputAction action;

    [SerializeField]
    FPSManager fpsManager;
    VFXDebugRuntimeView drv;

    private void Awake()
    {
        drv = GetComponent<VFXDebugRuntimeView>();
        drv.onDebugVisibilityChange += OnDebugVisibilityChange;
    }

    private void OnDebugVisibilityChange(bool visible)
    {
        fpsManager.SetActive(visible);
        Cursor.visible = visible;
        Cursor.lockState = visible? CursorLockMode.None : CursorLockMode.Locked;
    }
}
