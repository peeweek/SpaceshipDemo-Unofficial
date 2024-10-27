using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;
using DebugMenuUtility;
using System;
using GameplayIngredients.Controllers;
using UnityEngine.InputSystem.LowLevel;

[ManagerDefaultPrefab("FreeCamManager")]
public class FreeCamManager : Manager
{
    [NonNullCheck]
    public GameObject FreeCamera;

    public bool freeCameraEnabled
    {
        get { return FreeCamera.activeInHierarchy; }
        set
        {
            if (!FreeCamera.activeInHierarchy && value)
            {
                FreeCamera.transform.position = Manager.Get<VirtualCameraManager>().transform.position;
                FreeCamera.transform.forward = Manager.Get<VirtualCameraManager>().transform.forward;
            }


            FreeCamera.SetActive(value);
        }
    }

    [DebugMenuItem("")]
    public class ToggleFreeCamDebugMenuEntry : DebugMenuItem
    {
        public override string label => Manager.Get<FreeCamManager>().freeCameraEnabled ? "Disable Free Cam" : "Enable Free Cam";

        public override Action OnValidate => ToggleFreeCam;

        void ToggleFreeCam()
        {
            var fcm = Manager.Get<FreeCamManager>();
            fcm.freeCameraEnabled = !fcm.freeCameraEnabled;

            var players = FindObjectsOfType<FirstPersonController>();
            foreach (var p in players)
            {
                p.Paused = fcm.freeCameraEnabled;
            }

        }
    }

}
