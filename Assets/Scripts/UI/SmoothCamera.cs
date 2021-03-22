using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public float smoothness = 0.15f;
    
    private Vector3 velocity = Vector3.zero;

    private float _shakeDecay;
    private float _currentShakeIntensity;
    private Quaternion _originalRotation;

    private bool _resetRotation;

    private void Start()
    {
        _currentShakeIntensity = 0;
        _originalRotation = Quaternion.identity;
        _resetRotation = false;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        if (_resetRotation)
        {
            transform.rotation = _originalRotation;
            _resetRotation = false;
        }
        if (_currentShakeIntensity <= 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothness);
        }
        else
        {
            transform.position = desiredPosition + _getShakePosition();
            transform.rotation = _getShakeRotation();
            _currentShakeIntensity -= _shakeDecay;
            if (_currentShakeIntensity <= 0)
                _resetRotation = true;
        }
    }

    private Vector3 _getShakePosition()
    {
        return Random.insideUnitSphere * _currentShakeIntensity;
    }

    private Quaternion _getShakeRotation()
    {
        return new Quaternion(
            _originalRotation.x + Random.Range(-_currentShakeIntensity,_currentShakeIntensity)*.2f,
            _originalRotation.y + Random.Range(-_currentShakeIntensity,_currentShakeIntensity)*.2f,
            _originalRotation.z + Random.Range(-_currentShakeIntensity,_currentShakeIntensity)*.2f,
            _originalRotation.w + Random.Range(-_currentShakeIntensity,_currentShakeIntensity)*.2f);
    }

    public bool IsShaking()
    {
        return _currentShakeIntensity > 0;
    }
    
    public enum WiggleForce
    {
        Low,
        Medium,
        High
    }

    public void WiggleCamera(WiggleForce force)
    {
        if (force == WiggleForce.Low)
            _wiggle(0.05f, 0.0005f);
        if (force == WiggleForce.Medium)
            _wiggle(0.1f, 0.001f);
        if (force == WiggleForce.High)
            _wiggle(0.2f, 0.002f);
    }

    private void _wiggle(float force, float decay)
    {
        if (!IsShaking())
            _originalRotation = transform.rotation;
        _currentShakeIntensity = force;
        _shakeDecay = decay;
    }
}
