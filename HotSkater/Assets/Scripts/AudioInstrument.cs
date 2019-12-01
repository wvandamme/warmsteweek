using System;
using System.Collections;
using DG.Tweening;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using Valve.Newtonsoft.Json.Utilities;

[Serializable]
public class SoundLoop
{
    public Note Note;
    
    public AudioClip[] Audio;
}

public class AudioInstrument : MonoBehaviour
{
    public static AudioInstrument ActiveInstrument { get; private set; }

    
    public SoundLoop[] Loops;
    
    private AudioClip _activeClip;

    private Collider _collider;
    private Rigidbody _rigidbody;
    private Tween _activeTween;

    /// ===============================================================================================
    /// Events
    public delegate void AudioInstrumentEventHandler(AudioInstrument sender);
    
    public static event AudioInstrumentEventHandler GlobalSwitchPhase;
    public static event AudioInstrumentEventHandler GlobalInstrumentHit;

    /// ===============================================================================================
    /// Methods
    public void Start()
    {
        ActiveInstrument = null;
        
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody)
            _collider = GetComponentInChildren<Collider>();
        else _collider = GetComponent<Collider>();
        
        if (_collider == null || !_collider.isTrigger)
            Debug.LogError($"{name} is missing a trigger collider!", gameObject);
    }

    private void OnEnable()
    {
        AudioInstrument.GlobalInstrumentHit += OnInstrumentHit;
        Note.GlobalNoteHit += OnNoteHit;
    }

    private void OnDisable()
    {
        Note.GlobalNoteHit -= OnNoteHit;
        AudioInstrument.GlobalInstrumentHit -= OnInstrumentHit;
    }
    
    private void OnInstrumentHit(AudioInstrument sender)
    {
        ToggleCollider(false);

        _activeTween?.Complete();
        
        _activeTween = transform.DOScale(Vector3.zero, 1f);

        if (this == sender)
        {
            _activeTween.SetDelay(1f);
            _activeTween.SetEase(Ease.InBounce);
        }
        else
            _activeTween.SetEase(Ease.InOutBounce);
    }

    private void OnNoteHit(Note sender)
    {
        _activeTween?.Complete();
        
        _activeTween = transform.DOScale(Vector3.one, .5f);
        _activeTween.SetEase(Ease.OutBounce);
        
        _activeTween.onComplete += () => ToggleCollider(true);
        
    }
    
    private void ToggleCollider(bool state)
    {
        _collider.enabled = state;
    }

#if ODIN_INSPECTOR
    [Button]
#endif
    public void TriggerLoop(int index)
    {
        SoundManager.Instance.EnqueueLoop(this, index);
    }
    
    public void TriggerLoop(Note note)
    {
        var index = Loops.IndexOf(x => x.Note == note);
        if (index == -1)
        {
            Debug.LogError($"Could not find Note {note.name} in instrument {name}");
            return;
        }
        SoundManager.Instance.EnqueueLoop(this, index);
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

            OnSwitchPhase();

            index = (index + 1) % loop.Audio.Length;
        }
    }

    private void OnSwitchPhase()
    {
        GlobalSwitchPhase?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ActiveInstrument = this;
            GlobalInstrumentHit?.Invoke(this);
            Debug.Log($"Hit {name}");
        }
    }
}