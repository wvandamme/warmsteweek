using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Scooter : MonoBehaviour
{
    public bool ExecuteInEditMode = true;
    public Transform FollowObject;

    public void Update()
    {
        if (FollowObject != null)
            HandleFollowObject();
    }

    private void HandleFollowObject()
    {
        if (!Application.isPlaying && !ExecuteInEditMode)
            return;
        
        
    }
}
