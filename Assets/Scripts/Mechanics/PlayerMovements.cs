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
    
    private Rigidbody2D _body;
    private Transform _spawnPoint;

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
    }

    // Update is called once per frame
    void Update()
    {
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

        Debug.Log("State: " + _state);
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.tag == "Ground")
                {
                    _state = State.Grounded;
                    return;
                }
            }
        }
        if (_body.velocity.y > 0)
            _state = State.Ascending;
        else if (_body.velocity.y < 0)
            _state = State.Falling;
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
        _jumpDeltaTime += Time.deltaTime;

        if (_state == State.Grounded && _jumpDeltaTime > jumpDelay && Input.GetAxisRaw("Vertical") > 0)
        {
            Debug.Log("Jumping.");
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
}
