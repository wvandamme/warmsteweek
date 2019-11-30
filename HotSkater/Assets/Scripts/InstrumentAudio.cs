using System.Collections;
using System.Linq;
using UnityEngine;

public struct InstrumentAudio
{
    public AudioInstrument Instrument;
    public int LoopIndex;

    public void Start()
    {
        if (Instrument.Loops.Length <= LoopIndex || LoopIndex < 0)
        {
            Debug.LogError("Invalid Index for instrument");
            return;
        }
        
        var go = new GameObject($"Loop {LoopIndex}");
        go.transform.SetParent(Instrument.transform);
        var source = go.AddComponent<AudioSource>();
        
        Instrument.StartLoop(source, LoopIndex);
    }
}