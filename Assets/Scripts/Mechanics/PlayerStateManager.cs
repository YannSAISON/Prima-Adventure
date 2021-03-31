using System.Collections;
using System.Collections.Generic;
using Mechanics;
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

    private bool _isEnabled;

    private ParticleSystem _particle;

    private AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindObjectOfType<SmoothCamera>();
        _playerMovements = gameObject.GetComponent<PlayerMovements>();
        _swag = swagMax;
        _currentHealth = health;
        _isEnabled = true;
        _particle = gameObject.GetComponentInChildren<ParticleSystem>();
        _audio = gameObject.GetComponent<AudioSource>();
        _resetEnemies();
        //TODO Reset enemies.
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isEnabled)
            return;
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
        if (!_isEnabled)
            return;
        _currentHealth -= damages;
        if (_currentHealth < 0)
        {
            _camera.WiggleCamera(SmoothCamera.WiggleForce.High);
            StartCoroutine("PlayerDeathAnimation");
        }
    }

    public IEnumerator PlayerDeathAnimation()
    {
        _isEnabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
        _particle.Play();
        _audio.Play();
        // _playerMovements.isEnabled = false;
        _playerMovements.Disable();
        while (_particle.isPlaying) 
            yield return new WaitForSeconds(0.1f);
        _playerMovements.MoveToSpawn();
        gameObject.GetComponent<Renderer>().enabled = true;
        _currentHealth = health;
        _swag = swagMax;
        // _playerMovements.isEnabled = false;
        _playerMovements.Enable();
        _resetEnemies();
        _isEnabled = true;
    }

    private void _resetEnemies()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objects)
        {
            BasicEnemyController controller = obj.GetComponentInChildren<BasicEnemyController>();
            if (controller != null)
                controller.Enable();
        }
    }
}
