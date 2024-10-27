using UnityEngine;

public class ShaderVariantWarmup : MonoBehaviour
{
    public ShaderVariantCollection shaderVariants;
    void Start()
    {
        shaderVariants.WarmUp();
    }
}
