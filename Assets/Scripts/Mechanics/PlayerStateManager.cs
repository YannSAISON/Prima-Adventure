using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public int health = 100;
    private int _currentHealth;
    public int damages = 50;
    
    public int swagMax = 100;

    public float swagPerSec = 1f;
    private int _swag;
    private float _swagDeltaT = 0f;

    public int dashCost = 20;
    
    private SmoothCamera _camera;

    private PlayerMovements _playerMovements;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindObjectOfType<SmoothCamera>();
        _playerMovements = gameObject.GetComponent<PlayerMovements>();
        _swag = swagMax;
        _currentHealth = health;
        //TODO Reset enemies.
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerMovements.isDashing && Input.GetButtonDown("Fire1") && _swag >= dashCost)
        {
            _playerMovements.StartDash();
            _swag -= dashCost;
        }

        _swagDeltaT -= Time.deltaTime;
        if (_swagDeltaT < 0)
        {
            if (_swag < swagMax)
                _swag++;
            _swagDeltaT = 1 / swagPerSec;
        }
    }

    public void Killed(int swagBack)
    {
        _swag += swagBack;
        _swag = _swag > swagMax ? swagMax : _swag;
    }
    
    public void Hit(int damages)
    {
        _currentHealth -= damages;
        if (_currentHealth < 0)
        {
            _camera.WiggleCamera(SmoothCamera.WiggleForce.High);
            _playerMovements.MoveToSpawn();
            _currentHealth = health;
            _swag = swagMax;
            //TODO Reset enemies.
        }
    }
}
