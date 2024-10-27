using UnityEngine;
using DebugMenuUtility;
using System;
using UnityEngine.VFX.DebugTools;

[DebugMenuItem("Debug")]
public class DebugMenuItem_EnableVFXStatistics : DebugMenuItem
{
    public override string label => throw new System.NotImplementedException();
    public override string value => VFXDebugRuntimeView.instance.visible? "Visible" : "Hidden";
    public override Action OnValidate => Toggle;

    void Toggle()
    {
        VFXDebugRuntimeView.instance.visible = !VFXDebugRuntimeView.instance.visible;
        DebugMenu.Close();
    }

}
