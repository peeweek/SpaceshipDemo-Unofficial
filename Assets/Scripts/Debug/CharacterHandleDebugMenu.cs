using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayIngredients;
using GameplayIngredients.Controllers;

using ConsoleUtility;
using DebugMenuUtility;


[RequireComponent(typeof(FirstPersonController))]
public class CharacterHandleDebugMenu : MonoBehaviour
{
    FirstPersonController fpsController;

    private void OnEnable()
    {
        fpsController = GetComponent<FirstPersonController>();

        Debug.Assert(fpsController != null);

        Console.onConsoleToggle += Console_onConsoleToggle;
        DebugMenu.onDebugMenuToggle += DebugMenu_onDebugMenuToggle;
    }

    private void DebugMenu_onDebugMenuToggle(bool visible)
    {
        fpsController.Paused = visible;
    }

    private void Console_onConsoleToggle(bool visible)
    {
        fpsController.Paused = visible;
    }
}
