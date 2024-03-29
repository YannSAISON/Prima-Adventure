﻿using System;
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
    public enum State
    {
        Grounded,
        Ascending,
        Falling,
    }
    
    public float horizontalSpeed = 5;
    public float jumpForce = 5;
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

    private State _state;
    public float jumpDelay = 0.1f;
    private float _jumpDeltaTime;
    SpriteRenderer spriteRenderer;

    private Rigidbody2D _body;
    internal Animator animator;
    private Transform _spawnPoint;

    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        isDashing = false;
        _speed = Vector2.zero;
        _movement = Vector2.zero;
        _body = GetComponent<Rigidbody2D>();
        _dashDestination = Vector2.zero;
        _spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint").GetComponent<Transform>();
        _body.position = _spawnPoint.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
            return;
        if (!isDashing)
        {
            _updateState();
            _defineBasicMovement();
            _applyBasicSpeed(Time.deltaTime);
        }
        else
        {
            _updateDash();
        }
    }

    public void StartDash()
    {
        if (isDashing)
            return;
        _startDash();
    }
    
    private void _updateState()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(_body.position, Vector2.down, 1.30f);
        Debug.DrawRay(_body.position, Vector2.down * 1.30f, Color.red);

        // Debug.Log("State: " + _state);
        if (_body.velocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (_body.velocity.x < -0.01f)
            spriteRenderer.flipX = true;
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.tag == "Ground")
                {
                    _state = State.Grounded;
                    //Debug.Log("Velocity X = " + Mathf.Abs(_body.velocity.x) / horizontalSpeed);
                    animator.SetBool("grounded", true);
                    animator.SetFloat("velocityX", Mathf.Abs(_body.velocity.x) / horizontalSpeed);
                    return;
                }
            }
        }
        animator.SetBool("grounded", false);
        if (_body.velocity.y > 0)
            _state = State.Ascending;
        else if (_body.velocity.y < 0)
            _state = State.Falling;
        //Debug.Log("Velocity X = " + Mathf.Abs(_body.velocity.x) / horizontalSpeed);
        animator.SetFloat("velocityX", Mathf.Abs(_body.velocity.x) / horizontalSpeed);
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
            Debug.Log("Done");
            _body.AddForce(_dashVector * _speed);
            _movement = _dashVector;
            _speed = Vector2.one * dashSpeed * (Time.deltaTime * (_dashDuration - (_dashSumDeltaT + Time.deltaTime)));
            animator.SetBool("grounded", false);
            isDashing = false;
        }
        else
        {
            Debug.Log("Dashing");
            _movement = _dashVector;
            animator.SetBool("grounded", true);
            _speed = Vector2.one * dashSpeed;
        }
        
        _body.velocity = _movement * _speed;
        _dashSumDeltaT += Time.deltaTime;
    }

    public static Vector2 ClampMagnitude(Vector2 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        else if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }

    private void _defineDashDestination()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = mousePosition - _body.position;
        float distanceSq = diff.magnitude; /*Mathf.Sqrt(Mathf.Pow(
                                          diff.x, 2) +
                                      Mathf.Pow(
                                          diff.y, 2));*/
        float range = 0;
        _dashVector = diff / distanceSq;
        if (distanceSq == 0)
        {
            Debug.Log("AH");
            isDashing = false;
            return;
        }
        else if (minDashRange <= distanceSq && distanceSq <= maxDashRange)
        {
            Debug.Log("Classic Dash");
            _dashDestination = mousePosition;
            range = distanceSq;
        }
        else
        {
            Debug.Log("distanceSQ = [" + distanceSq + "]");
            float multiplier = (distanceSq > maxDashRange ? maxDashRange : minDashRange);
            _dashDestination = _body.position + ClampMagnitude(diff, maxDashRange, minDashRange);/*_body.position + _dashVector * multiplier*/;
            range = multiplier;
        }

        _dashSumDeltaT = 0;
        _dashDuration = range / dashSpeed;
            Debug.Log("diff = [" + diff.magnitude + "]");
    }

    private float _getSign(float nb)
    {
        return nb < 0 ? -1 : 1;
    }

    private void _defineBasicMovement()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _speed.x = horizontalSpeed;
        _jumpDeltaTime += Time.deltaTime;

        if (_state == State.Grounded && _jumpDeltaTime > jumpDelay && Input.GetAxisRaw("Vertical") > 0)
        {
            // Debug.Log("Jumping.");
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _jumpDeltaTime = 0;
        }
        //TODO Jump
    }

    private void _applyBasicSpeed(float deltaTime)
    {
        _body.velocity = _movement * _speed * Vector2.right + _body.velocity * Vector2.up;
        _movement = Vector2.zero;
        _speed = Vector2.zero;
    }

    public void MoveToSpawn()
    {
        _body.position = _spawnPoint.position;
    }

    public void Disable()
    {
        isEnabled = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        _body.velocity = Vector2.zero;
        _movement = Vector2.zero;
        _speed = Vector2.zero;
    }
    
    public void Enable()
    {
        isEnabled = true;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;
    }
}
