using System;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public float smoothness = 0.15f;
    
    private Vector3 velocity = Vector3.zero;
    
    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothness);
    }
}
