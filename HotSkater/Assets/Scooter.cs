using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Scooter : MonoBehaviour
{
    public LayerMask RaycastMask;
    
    public bool ExecuteInEditMode = true;
    public Transform FollowObject;

    private Vector3 _prevPosition;
    private Vector3 _targetPosition;
    private Vector3 _prevForward;
    private Vector3 _targetForward;

    public float MoveSpeed = 1f;
    public float RotationSpeed = 45f;

    public void Start()
    {
        _targetPosition = transform.position;
        _targetForward = transform.forward;
    }
    
    public void Update()
    {
        if (FollowObject != null)
            HandleFollowObject();
    }

    private void HandleFollowObject()
    {
        if (!Application.isPlaying && !ExecuteInEditMode)
            return;

        var position = FollowObject.position;
        position.y = 3f;
        var forward = (position - _prevPosition).normalized;

        var dir = -Vector3.up;
        var ray = new Ray(position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, RaycastMask))
        {
            _targetPosition = hit.point;
            if ((position - _prevPosition).magnitude > float.Epsilon * 50)
                _targetForward = forward;

        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * MoveSpeed);
        if (_targetForward.sqrMagnitude > 0)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_targetForward), Time.deltaTime * RotationSpeed);

        _prevPosition = position;
        _prevForward = forward;
    }
}
