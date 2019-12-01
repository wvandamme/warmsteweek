using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public Transform[] Objects;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Objects == null) return;
        foreach (var o in Objects)
        {
            
            var color = Color.HSVToRGB(Random.Range(0f, 1f), .8f, .8f);
            var renderer = o.GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.material.SetColor("_Color", color);
            o.GetComponentInChildren<Light>().color = color;
        }
    }
}
