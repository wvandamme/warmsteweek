using System;
using System.Collections;
using System.Collections.Generic;
using Rhinox.Utilities;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioInstrument[] Instruments;

    private List<InstrumentAudio> _activeLoops;

    private void Start()
    {
        _activeLoops = new List<InstrumentAudio>();
    }

    public void StartLoop(AudioInstrument instrument, int index)
    {
        var loop = new InstrumentAudio
        {
            Instrument = instrument,
            LoopIndex = index
        };
        _activeLoops.Add(loop);
        
        loop.Start();
    }
}
