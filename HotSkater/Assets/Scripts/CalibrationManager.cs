﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public struct CalibrationData
{
    public Vector3 Position;
    public Vector3 Direction;
}

public class CalibrationManager : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public bool Active { get; private set; }
    
    private const string CalibrationFile = "Calibration";
    private string CalibrationFilePath => Path.Combine(Application.dataPath, CalibrationFile);
    public Transform ReferencePoint;

    public Transform CalibrationRoot;
    
    public SteamVR_Action_Boolean CalibrationAction; // Choose grab pinch in inspector
    private SteamVR_Input_Sources _inputSource = SteamVR_Input_Sources.Any;

    /// ===============================================================================================
    /// Calibration State
    [NonSerialized] public CalibrationData CalibrationData;

    public bool DisableCalibrationWhenDone = true;

    public bool CalibrationNeeded => CalibrationData.Equals( default(CalibrationData) );

    private bool _isCalibrating;
    
    /// ===============================================================================================
    /// Methods
    
    
    void Start()
    {
        LoadCalibration();

        if (CalibrationNeeded && !Active)
            ToggleActive();
        else SetActive(false);
        
        CalibrationAction.AddOnChangeListener(OnCalibrationButtonTriggered, _inputSource);
    }

    void LoadCalibration()
    {
        if (!File.Exists(CalibrationFilePath)) return;
        
        var json = File.ReadAllText(CalibrationFilePath);

        CalibrationData = JsonUtility.FromJson<CalibrationData>(json);
        
        Debug.Log($"Loaded Calibration: {json}");

        ApplyCalibration();
    }

    void SaveCalibration()
    {
        var json = JsonUtility.ToJson(CalibrationData);
        
        File.WriteAllText(CalibrationFilePath, json);
        
        Debug.Log($"Saved Calibration: {json}");

        if (DisableCalibrationWhenDone)
            SetActive(false);

        ApplyCalibration();
    }

    void ApplyCalibration()
    {
        CalibrationRoot.position = -ReferencePoint.TransformPoint( CalibrationData.Position );
        CalibrationRoot.forward = CalibrationData.Direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift))
            ToggleActive();

        if (!Active) return;
            
    }

    private void ToggleActive()
    { 
        if (CalibrationNeeded && Active)
            return;
        
        SetActive(!Active);
    }

    private void SetActive(bool newState)
    {
        Active = newState;
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(Active);
        }
        
        if (!Active)
            LoadCalibration();
    }
    
    void OnGUI()
    {
        if (!Active) return;
        
        if (CalibrationNeeded)
        {
            var oldColor = GUI.color;
            GUI.color = Color.red;
            GUILayout.Space(10);
            GUILayout.Label("CALIBRATION NEEDED");
            GUI.color = oldColor;
        }
        
        GUILayout.Space(20);
        
        GUILayout.Label("Press Trigger on circle at instruments to calibrate");
    }

    private void OnCalibrationButtonTriggered(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource, bool newstate)
    {
        if (!Active) return;

        // Reset root to calibrate
        if (!_isCalibrating)
        {
            CalibrationRoot.position = Vector3.zero;
            CalibrationRoot.rotation = Quaternion.identity;
        }

        var source = fromaction.GetActiveDevice(fromsource);
        var device = SteamVR_Behaviour_Pose.GetPose(source);
        // var targetPos = device.transform.position;
        var targetPos = ReferencePoint.InverseTransformPoint( device.transform.position );
        targetPos.y = 0;

        if (fromaction.stateDown)
        {
            CalibrationData.Position = targetPos;
            
            _isCalibrating = true;
        }
        else if (fromaction.stateUp)
        {
            var direction = targetPos - CalibrationData.Position;
            direction.Normalize();

            CalibrationData.Direction = direction;
            
            _isCalibrating = false;
            
            SaveCalibration();
        }
    }

    private void OnDrawGizmosSelected()
    {
        var pos = CalibrationData.Position;
        pos.y = .4f;
        Gizmos.DrawWireSphere(pos, .1f);
        Gizmos.DrawLine(pos, CalibrationData.Position + CalibrationData.Direction);
    }

    private void OnDrawGizmos()
    {
        if (!_isCalibrating) return;
        
        var pos = CalibrationData.Position;
        pos.y = .4f;
        Gizmos.DrawWireSphere(pos, .3f);
    }
}
