using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rhinox.Utilities;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public float SoundDampeningPercentage = .2f;
    
    private List<InstrumentAudio> _activeLoops;
    private InstrumentAudio _masterAudio;

    public Queue<InstrumentAudio> _queuedLoops;
    
    private void Start()
    {
        _activeLoops = new List<InstrumentAudio>();
        _queuedLoops = new Queue<InstrumentAudio>();

        AudioInstrument.GlobalSwitchPhase += OnSwitchPhase;
    }

    private void OnSwitchPhase(AudioInstrument sender)
    {
        if (_masterAudio == null || sender != _masterAudio.Instrument)
            return;
        
        DequeueLoop();
    }

    private void Update()
    {
        _masterAudio = _activeLoops.FirstOrDefault(x => x.Source != null);

        if (_masterAudio == null) return;

        foreach (var loop in _activeLoops)
        {
            if (loop.Source == null || loop.Source == _masterAudio.Source)
                return;

            loop.Source.timeSamples = _masterAudio.Source.timeSamples;
        }
    }
    
    public void EnqueueLoop(AudioInstrument instrument, int index)
    {
        var loop = new InstrumentAudio
        {
            Instrument = instrument,
            LoopIndex = index
        };
        if (_masterAudio == null)
            StartLoop(loop);
        else
            _queuedLoops.Enqueue(loop);
    }

    private void DequeueLoop()
    {
        if (_queuedLoops.Count == 0) return;
        
        // Dim existing loops
        foreach (var activeLoop in _activeLoops)
            activeLoop.Source.volume *= 1 - SoundDampeningPercentage;
        
        // Start new loop
        var loop = _queuedLoops.Dequeue();
        StartLoop(loop);
    }

    private void StartLoop(InstrumentAudio loop)
    {
        _activeLoops.Add(loop);
        loop.Start();
    }
}
