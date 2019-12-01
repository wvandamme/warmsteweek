using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

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
    
    public SteamVR_Action_Boolean CalibrationAction; // Choose grab pinch in inspector
    private SteamVR_Input_Sources _inputSource = SteamVR_Input_Sources.Any;

    /// ===============================================================================================
    /// Calibration State
    public CalibrationData CalibrationData;

    public bool CalibrationNeeded => CalibrationData.Equals( default(CalibrationData) );

    private bool _isCalibrating;
    
    /// ===============================================================================================
    /// Methods
    
    
    void Start()
    {
        LoadCalibration();
        
        if (CalibrationNeeded && !Active)
            ToggleActive();
        
        CalibrationAction.AddOnChangeListener(OnCalibrationButtonTriggered, _inputSource);
    }

    void LoadCalibration()
    {
        if (!File.Exists(CalibrationFilePath)) return;
        
        var json = File.ReadAllText(CalibrationFilePath);

        CalibrationData = JsonUtility.FromJson<CalibrationData>(json);
        
        // TODO apply loaded data
    }

    void SaveCalibration()
    {
        var lines = new List<string>();
        var json = JsonUtility.ToJson(CalibrationData);
        
        File.WriteAllText(CalibrationFilePath, json);
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
        
        Active = !Active;
        
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

        if (fromaction.stateDown)
        {
            var device = fromaction.GetActiveDevice(_inputSource);

            _isCalibrating = true;
            
            // SteamVR_Action_Pose.
            
            //SteamVR_Action_Boolean.
            // var device = fromaction[_inputSource];
            Debug.Log(device);
            // CalibrationPoints.Add(fromaction.trackedDeviceIndex);
        }
        else if (fromaction.stateUp)
        {
            _isCalibrating = false;
        }
    }
}
