using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushCoord : MonoBehaviour
{
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        if(material == null)
        {
            Debug.Log("Material null");
            material = new Material(Shader.Find("Standard"));
            material.color = Color.yellow;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) { 
            renderer.material = material;
        }
    }
}
