using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Note : MonoBehaviour
{
    public static List<Note> Notes = new List<Note>();
    
    private Collider _collider;
    private Rigidbody _rigidbody;
    private Tween _activeTween;

    private SkinnedMeshRenderer _renderer;
    private Light _light;
    private float _lightIntensity;

    private bool _active;
    
    public delegate void NoteEventHandler(Note sender);
    
    public static event NoteEventHandler GlobalNoteHit;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody)
            _collider = GetComponentInChildren<Collider>();
        else _collider = GetComponent<Collider>();
        
        if (_collider == null || !_collider.isTrigger)
            Debug.LogError($"{name} is missing a trigger collider!", gameObject);

        // Get Params for RandomizeColor
        _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _light = GetComponentInChildren<Light>();
        _lightIntensity = _light.intensity;
        _light.intensity = 0f;
        
        // Disable collider & scale at startup
        ToggleCollider(false);
        transform.localScale = Vector3.zero;
        _active = false;
    }

    private void RandomizeColor()
    {
        var color = Color.HSVToRGB(Random.Range(0f, 1f), .8f, .8f);
        _renderer.material.SetColor("_Color", color);
        _light.color = color;
    }

    private void OnEnable()
    {
        Notes.Add(this);
        
        AudioInstrument.GlobalInstrumentHit += OnInstrumentHit;
        Note.GlobalNoteHit += OnNoteHit;
    }

    private void OnDisable()
    {
        Notes.Remove(this);
        
        AudioInstrument.GlobalInstrumentHit -= OnInstrumentHit;
        Note.GlobalNoteHit -= OnNoteHit;
    }

    private void OnInstrumentHit(AudioInstrument sender)
    {
        if (sender.Loops.All(x => x.Note != this))
            return;

        _active = true;
        
        RandomizeColor();
        
        ToggleCollider(true);

        _activeTween?.Complete();

        _activeTween = transform.DOScale(Vector3.one, .5f);
        _activeTween.SetEase(Ease.OutBounce);

        _light.DOIntensity(_lightIntensity, .2f);
    }

    private void ToggleCollider(bool state)
    {
        _collider.enabled = state;
    }

    private void OnNoteHit(Note sender)
    {
        if (!_active) return;
        
        ToggleCollider(false);
        
        _activeTween?.Complete();
        
        _activeTween = transform.DOScale(Vector3.zero, 1f);

        if (this == sender)
        {
            _activeTween.SetDelay(1f);
            _activeTween.SetEase(Ease.InBounce);
            var tween = _light.DOIntensity(0, .4f);
            tween.SetEase(Ease.OutBounce, 3f);
        }
        else
        {
            _activeTween.SetEase(Ease.InOutBounce);
            _light.DOIntensity(0, .2f);
        }
        
        AudioInstrument.ActiveInstrument.TriggerLoop(this);

        _active = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            
            GlobalNoteHit?.Invoke(this);
            Debug.Log($"Hit {name}");
            
        }
    }
}
