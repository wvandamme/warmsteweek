using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMaterial : MonoBehaviour
{
    public float ScrollSpeed = 10;

    private MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        foreach (var mat in _renderer.sharedMaterials)
        {
            var offset = mat.mainTextureOffset;
            offset.x += Time.deltaTime * ScrollSpeed;
            mat.mainTextureOffset = offset;
        }
        
    }
}
