using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

namespace Mechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class BasicEnemyController : MonoBehaviour
    {
        // public Vector2 add;

        public enum EnemyState
        {
            Hiking,
            Hunting,
        }

        public int maxHealth;
        private int _health;
        public int swagBack = 20;

        private float _speed;
        private float _chasingSpeed;
        private Vector2 _originalPosition;
        private Vector2 _destination;
        private bool _toDest;
        private EnemyState _state;

        private Collider2D _collider;

        private Transform _player;

        private PlayerStateManager _playerState;
        private int _damages;
        SpriteRenderer spriteRenderer;
        public float timeBetweenHits;
        private float _timeRemaining;

        private SmoothCamera _camera;

        private ParticleSystem _particle;
        private AudioSource _audio;
        
        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponentInParent<EnemyAreaController>()
                .Init(out _originalPosition, out _destination, out _speed, out _chasingSpeed, out _damages);
            _toDest = true;
            _health = maxHealth;
            _state = EnemyState.Hiking;
            GameObject player = GameObject.Find("_Player");
            _player = player.GetComponent<Transform>();
            if (_player == null)
                throw new Exception("Player not found.");
            _playerState = player.GetComponent<PlayerStateManager>();
            _camera = GameObject.FindObjectOfType<SmoothCamera>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _particle = transform.GetComponentInChildren<ParticleSystem>();
            _audio = transform.GetComponentInChildren<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.GetComponent<Renderer>().enabled == false || !other.CompareTag("Player"))
                return;
            PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
            if (playerMovement != null && playerMovement.isEnabled)
            {
                if (playerMovement.isDashing)
                    Hit(_playerState.damages);
                else
                {
                    _timeRemaining = timeBetweenHits;
                    _playerState.Hit(_damages);
                }
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
            if (!other.CompareTag("Player") || playerMovement == null || !playerMovement.isEnabled)
                return;
            _timeRemaining -= Time.deltaTime;
            Debug.Log("Still inside.");
            if (_timeRemaining <= 0)
            {
                Debug.Log("New hit.");
                _timeRemaining = timeBetweenHits;
                _playerState.Hit(_damages);
            }
        }

        public void Hit(int damages)
        {
            _health -= damages;
            if (_health <= 0)
            {
                _health = 0;
                _destroy();
            }
        }

        private void _destroy()
        {
            // Debug.Log("Destroying enemy.");
            _playerState.Killed(swagBack);
            //TODO Hide object
            gameObject.GetComponent<Renderer>().enabled = false;
            //TODO Trigger particles
            _particle.Play();
            //TODO Trigger sound
            _audio.Play();
            //// TODO Wiggle camera
            // _camera.WiggleCamera(SmoothCamera.WiggleForce.Low);
            ////TODO Wait to destroy object
        }

        public void OnPlayerEnterArea()
        {
            _state = EnemyState.Hunting;
        }

        public void OnPlayerExitArea()
        {
            _state = EnemyState.Hiking;
        }
        
        // Update is called once per frame
        void Update()
        {
            Debug.DrawLine(_originalPosition, _destination, Color.green);
            if (_state == EnemyState.Hiking)
            {
                _handleHikingState();
                spriteRenderer.flipX = !_toDest;
            } else if (_state == EnemyState.Hunting)
            {
                _handleHuntingState();
            }
        }

        private void _handleHikingState()
        {
            if (_toDest)
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position,
                    _destination, Time.deltaTime * _speed);
                _isCloseEnough(_destination);
            }
            else
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position,
                    _originalPosition, Time.deltaTime * _speed);
                _isCloseEnough(_originalPosition);
            }
        }

        private void _handleHuntingState()
        {
            if (_player.position.x - this.transform.position.x > 0)
            {
                Vector3 position = transform.position;
                Vector2 diff = _destination - (Vector2)position;
                if (diff.magnitude < .5)
                    return;
                this.transform.position = Vector2.MoveTowards(position,
                    _destination, Time.deltaTime * _chasingSpeed);
                spriteRenderer.flipX = false;
            }
            else
            {
                Vector3 position = transform.position;
                Vector2 diff = _originalPosition - (Vector2)position;
                if (diff.magnitude < .5)
                    return;
                spriteRenderer.flipX = true;
                this.transform.position = Vector2.MoveTowards(this.transform.position,
                    _originalPosition, Time.deltaTime * _chasingSpeed);
            }
        }
        
        private void _isCloseEnough(Vector2 dest)
        {
            Vector2 diff = dest - (Vector2)this.transform.position;
            if (diff.magnitude < .5)
            {
                _toDest = !_toDest;
            }
        }

        public void Enable()
        {
            // Debug.Log("Reset enemy.");
            _health = maxHealth;
            gameObject.GetComponent<Renderer>().enabled = true;
        }
    }
}
