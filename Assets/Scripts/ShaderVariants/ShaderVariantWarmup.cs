using UnityEngine;

public class ShaderVariantWarmup : MonoBehaviour
{
    public ShaderVariantCollection shaderVariants;
    void Start()
    {
        if(!Application.isEditor)
            shaderVariants.WarmUp();
    }
}
