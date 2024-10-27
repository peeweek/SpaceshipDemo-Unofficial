using UnityEngine;
using DebugMenuUtility;
using GameplayIngredients;
using GameplayIngredients.Actions;

public class DebugMenuToggleAction : ActionBase
{
    public enum ActionType
    {
        Open, Close, Enable, Disable
    }

    public ActionType actionType;

    public override void Execute(GameObject instigator = null)
    {
        switch (actionType)
        {
            case ActionType.Open:
                DebugMenu.Open();
                break; 
            case ActionType.Close:
                DebugMenu.Close();
                break;
            case ActionType.Enable:
                DebugMenu.CanBeOpened = true;
                break;
            case ActionType.Disable:
                DebugMenu.CanBeOpened = false;
                DebugMenu.Close();
                break;
     
        }
    }
}
