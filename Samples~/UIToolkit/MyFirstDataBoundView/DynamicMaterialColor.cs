using UnityEngine;

public class DynamicMaterialColor : MonoBehaviour
{
    public Material material;
    public Color Color
    {
        set
        {
            material.color = value;
        }
    }
}
