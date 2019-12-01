using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

using Valve.VR;

public class CalibrationManager : MonoBehaviour
{
    public SteamVR_Action_Boolean CalibrationAction; // Choose grab pinch in inspector
    private SteamVR_Input_Sources _inputSource = SteamVR_Input_Sources.Any;
    
    
    private const string CalibrationFile = "Calibration";
    private string CalibrationFilePath => Path.Combine(Application.dataPath, CalibrationFile);
    [ShowInInspector, ReadOnly] public bool Active { get; private set; }

    public const int PointsNeeded = 8;
    private int _currentPointCalibrating;
    
    public List<Vector3> CalibrationPoints;

    public bool CalibrationNeeded => CalibrationPoints == null || CalibrationPoints.Count != PointsNeeded;
    
    void Start()
    {
        CalibrationPoints = new List<Vector3>();
        LoadCalibration();

        _currentPointCalibrating = -1;
        
        if (CalibrationNeeded && !Active)
            ToggleActive();
        
        CalibrationAction.AddOnChangeListener(OnCalibrationButtonTriggered, _inputSource);
    }

    void LoadCalibration()
    {
        if (!File.Exists(CalibrationFilePath)) return;
        
        var pointLines = File.ReadAllLines(CalibrationFilePath);
        foreach (var line in pointLines)
        {
            var numsStr = line.Split(new[] { ';' });
            if (numsStr.Length != 3)
            {
                Debug.LogError($"Invalid Point: '{line}'");
                continue;
            }

            var nums = numsStr.Select( float.Parse ).ToArray();
            var vec = new Vector3(nums[0], nums[1], nums[2]);
            CalibrationPoints.Add(vec);
        }
    }

    void SaveCalibration()
    {
        var lines = new List<string>();
        foreach (var point in CalibrationPoints)
        {
            var text = $"{point.x};{point.y};{point.z}";
            lines.Add(text);
        }
        
        File.WriteAllLines(CalibrationFilePath, lines);
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
        
        _currentPointCalibrating = -1;

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

        if (_currentPointCalibrating == -1)
        {
            GUILayout.Label("Press Trigger to calibrate");
        }
    }

    private void OnCalibrationButtonTriggered(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource, bool newstate)
    {
        if (!Active) return;

        if (_currentPointCalibrating == -1)
        {
            CalibrationPoints.Clear();
            _currentPointCalibrating = 0;
        }

        if (fromaction.stateDown)
        {
            var device = fromaction.GetActiveDevice(_inputSource);
            
            // SteamVR_Action_Pose.
            
            //SteamVR_Action_Boolean.
            // var device = fromaction[_inputSource];
            Debug.Log(device);
            // CalibrationPoints.Add(fromaction.trackedDeviceIndex);
        }
    }
}
