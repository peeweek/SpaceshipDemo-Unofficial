using GameplayIngredients.Controllers;
using UnityEngine;
using GameOptionsUtility;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class SpaceshipFPSPlayerInput : GameplayIngredients.Controllers.PlayerInput
{
    // Public Interface

    [Header("Behaviour")]
    public float LookExponent = 2.0f;
    [Range(0.0f, 0.7f)]
    public float MovementDeadZone = 0.15f;
    [Range(0.0f, 0.7f)]
    public float LookDeadZone = 0.15f;

    public InputAction MoveAxis;
    public InputAction LookAxis;

    public InputAction LookMouseAxis;

    // Private ~ Properties
    Vector2 m_Movement;
    Vector2 m_Look;

    public override Vector2 Look => m_Look;
    public override Vector2 Movement => m_Movement;
    public override ButtonState Jump => ButtonState.Released;

    SpaceshipOptions options;

    private void OnEnable()
    {
        MoveAxis.Enable();
        LookAxis.Enable();
        LookMouseAxis.Enable();
    }

    private void OnDisable()
    {
        LookAxis.Disable();
        MoveAxis.Disable();
        LookMouseAxis.Disable();
    }


    public override void UpdateInput()
    {
        if(options == null)
            options = GameOption.Get<SpaceshipOptions>();

        SpaceshipOptions.FPSKeys keys = options.fpsKeys;

        
        m_Movement = MoveAxis.ReadValue<Vector2>();

        if (m_Movement.magnitude < MovementDeadZone)
            m_Movement = Vector2.zero;

        

        m_Movement.x += (GetKey(keys.left) ? -1 : 0) + (GetKey(keys.right)   ? 1 : 0);
        m_Movement.y += (GetKey(keys.back) ? -1 : 0) + (GetKey(keys.forward) ? 1 : 0);

        float mag = m_Movement.sqrMagnitude;

        if (mag > 1)
            m_Movement.Normalize();

        Vector2 l = LookAxis.ReadValue<Vector2>();
        m_Look = l.normalized * Mathf.Pow(Mathf.Clamp01(Mathf.Clamp01(l.magnitude) - LookDeadZone) / (1.0f - LookDeadZone), LookExponent);
        m_Look += LookMouseAxis.ReadValue<Vector2>();
    }


    /// <summary>
    /// Translation from Old inputsystem to new one
    /// (TODO) Do something cleaner later
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    bool GetKey(KeyCode code)
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        switch (code)
        {
            case KeyCode.W: return kb.wKey.isPressed;
            case KeyCode.A: return kb.aKey.isPressed;
            case KeyCode.S: return kb.sKey.isPressed;
            case KeyCode.D: return kb.dKey.isPressed;
            case KeyCode.Z: return kb.zKey.isPressed;
            case KeyCode.Q: return kb.qKey.isPressed;
            case KeyCode.I: return kb.iKey.isPressed;
            case KeyCode.J: return kb.jKey.isPressed;
            case KeyCode.K: return kb.kKey.isPressed;
            case KeyCode.L: return kb.lKey.isPressed;
            default:
                return false;
        }
    }
}
