using UnityEngine;

namespace JH.DataBinding.Examples.ContainerBindingTest
{    

public class MaterialColorSetter : MonoBehaviour
{
    public Color Color
    {
        set
        {
            GetComponent<Renderer>().material.color = value;
        } 
    }
}

}
