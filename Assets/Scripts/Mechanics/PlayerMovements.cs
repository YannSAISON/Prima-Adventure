using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    public float horizontalSpeed = 5;
    public float dashSpeed = 18;
    public float maxDashRange;
    public float minDashRange;
    
    private Vector2 _speed;
    private Vector2 _movement;
    
    public bool isDashing { get; private set; }
    private Vector2 _dashDestination;
    private Vector2 _dashVector;
    private float _dashDuration;
    private float _dashSumDeltaT;

    private Rigidbody2D _body;

    // Start is called before the first frame update
    void Start()
    {
        isDashing = false;
        _speed = Vector2.zero;
        _movement = Vector2.zero;
        _body = GetComponent<Rigidbody2D>();
        _dashDestination = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing && Input.GetButtonDown("Fire1")/* && player.haveEnoughEnergyToDash()*/) //TODO Change it depending on player resources
        {
            _startDash();
        }
        else if (!isDashing)
        {
            _defineBasicMovement();
            _applyBasicSpeed(Time.deltaTime);
        }
        else
        {
            _updateDash();
        }
    }

    private void _startDash()
    {
        isDashing = true;
        _defineDashDestination();
    }

    private void _updateDash()
    {
        Debug.DrawLine(transform.position, _dashDestination, Color.green);
        if (_dashSumDeltaT + Time.deltaTime > _dashDuration)
        {
            _body.AddForce(_dashVector * _speed);
            _movement = _dashVector;
            _speed = Vector2.one * dashSpeed * (Time.deltaTime * (_dashDuration - (_dashSumDeltaT + Time.deltaTime)));
            isDashing = false;
        }
        else
        {
            _movement = _dashVector;
            _speed = Vector2.one * dashSpeed;
        }
        
        _body.velocity = _movement * _speed;
        _dashSumDeltaT += Time.deltaTime;
    }

    private void _defineDashDestination()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = mousePosition - _body.position;
        float distanceSq = Mathf.Sqrt(Mathf.Pow(
                                          diff.x, 2) +
                                      Mathf.Pow(
                                          diff.y, 2));
        float range = 0;
        _dashVector = diff / distanceSq;
        if (distanceSq == 0)
        {
            isDashing = false;
            return;
        }
        else if (minDashRange <= distanceSq && distanceSq <= maxDashRange)
        {
            _dashDestination = mousePosition;
            range = distanceSq;
        }
        else
        {
            float multiplier = (distanceSq > maxDashRange ? maxDashRange : minDashRange);
            _dashDestination = _body.position + _dashVector * multiplier;
            range = multiplier;
        }

        _dashDuration = range / dashSpeed;
        _dashSumDeltaT = 0;
    }

    private float _getSign(float nb)
    {
        return nb < 0 ? -1 : 1;
    }

    private void _defineBasicMovement()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _speed.x = horizontalSpeed;
        //TODO Jump
    }

    private void _applyBasicSpeed(float deltaTime)
    {
        _body.velocity = _movement * _speed * Vector2.right + _body.velocity * Vector2.up;
        _movement = Vector2.zero;
        _speed = Vector2.zero;
    }
}
