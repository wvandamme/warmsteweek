using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class InstrumentAudio
{
    public AudioInstrument Instrument;
    public int LoopIndex;

    public AudioSource Source { get; private set; }

    public void Start()
    {
        if (Instrument.Loops.Length <= LoopIndex || LoopIndex < 0)
        {
            Debug.LogError("Invalid Index for instrument");
            return;
        }
        
        var go = new GameObject($"Loop {LoopIndex}");
        go.transform.SetParent(Instrument.transform);
        Source = go.AddComponent<AudioSource>();
        
        Instrument.StartLoop(Source, LoopIndex);
    }
}