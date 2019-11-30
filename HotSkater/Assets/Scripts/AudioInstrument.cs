using System;
using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

[Serializable]
public class SoundLoop
{
    public AudioClip[] Audio;
}

public class AudioInstrument : MonoBehaviour
{
    public SoundLoop[] Loops;
    
    private AudioClip _activeClip;
#if ODIN_INSPECTOR
    [Button]
#endif
    public void TriggerLoop(int index)
    {
        SoundManager.Instance.StartLoop(this, index);
    }
    
    public void StartLoop(AudioSource source, int index)
    {
        var loop = Loops[index];
        
        StartCoroutine( ManageAudioLoop(source, loop) );
    }

    private IEnumerator ManageAudioLoop(AudioSource source, SoundLoop loop)
    {
        source.loop = true;

        int index = 0;
        while (true)
        {
            _activeClip = loop.Audio[index];

            source.clip = _activeClip;
            
            if (!source.isPlaying)
                source.Play();
            
            yield return new WaitForSeconds(_activeClip.length);

            index = (index + 1) % loop.Audio.Length;
        }
    }
}