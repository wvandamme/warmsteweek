using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CalibrationManager : MonoBehaviour
{
    private const string CalibrationFile = "Calibration";
    private string CalibrationFilePath => Path.Combine(Application.dataPath, CalibrationFile);
    [ShowInInspector, ReadOnly] public bool Active { get; private set; }

    public List<Vector3> CalibrationPoints;

    public bool CalibrationNeeded => CalibrationPoints == null || CalibrationPoints.Count == 0;
    
    void Start()
    {
        CalibrationPoints = new List<Vector3>();
        LoadCalibration();
        
        if (CalibrationNeeded && !Active)
            ToggleActive();
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
        
        if (CalibrationPoints == null || !CalibrationPoints.Any())
        {
            Debug.LogError("CalibrationNeeded");
        }

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
        
        
    }
}
