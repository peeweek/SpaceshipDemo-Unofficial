using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
public class FreeCamController : MonoBehaviour
{
    public Transform cameraTransform;

    public InputAction MoveAxis;
    public InputAction LookAxis;
    public InputAction ThrustAxis;
    public InputAction BoostButton;
    public InputAction SwapThrustButton;

    public float MoveSpeed = 8f;
    public float LookSpeed = 180f;
    public float BoostMultiplier = 3f;

    public float MinMaxPitch = 80f;

    public bool ThrustForward = false;

    private void OnEnable()
    {
        MoveAxis.Enable();
        LookAxis.Enable();
        ThrustAxis.Enable();
        BoostButton.Enable();
        SwapThrustButton.Enable();
    }

    private void OnDisable()
    {
        MoveAxis.Disable();
        LookAxis.Disable();
        ThrustAxis.Disable();
        BoostButton.Disable();
        SwapThrustButton.Disable();
    }

    private void LateUpdate()
    {
        if (SwapThrustButton.WasPressedThisFrame())
            ThrustForward = !ThrustForward;

        float multiplier = 1f;

        if (BoostButton.IsPressed()) multiplier = BoostMultiplier;

        var move = MoveAxis.ReadValue<Vector2>();
        var thrust = ThrustAxis.ReadValue<float>();
        var look = LookAxis.ReadValue<Vector2>();

        Vector3 moveVec = Vector3.zero;

        if(ThrustForward)
        {
            moveVec.x = move.x;
            moveVec.y = move.y;
            moveVec.z = thrust;
        }
        else
        {
            moveVec.x = move.x;
            moveVec.y = thrust;
            moveVec.z = move.y;
        }

        moveVec = moveVec * MoveSpeed * Time.deltaTime * multiplier;

        // Move
        transform.position += moveVec.x * transform.right + moveVec.y * transform.up + moveVec.z * transform.forward ;
        
        // Look : 
        Vector2 lookAngles = new Vector2(look.x, look.y) * LookSpeed * Time.deltaTime;

        //Yaw
        transform.Rotate(Vector3.up, lookAngles.x, Space.World);

        //Pitch
        var pitch = cameraTransform.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        pitch = Mathf.Clamp(pitch - lookAngles.y, -MinMaxPitch, MinMaxPitch);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0, 0);


    }


}
