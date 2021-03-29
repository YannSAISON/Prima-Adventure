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

        public int health;
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

        private SmoothCamera _camera;
        
        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponentInParent<EnemyAreaController>()
                .Init(out _originalPosition, out _destination, out _speed, out _chasingSpeed, out _damages);
            _toDest = true;
            _state = EnemyState.Hiking;
            GameObject player = GameObject.Find("_Player");
            _player = player.GetComponent<Transform>();
            if (_player == null)
                throw new Exception("Player not found.");
            _playerState = player.GetComponent<PlayerStateManager>();
            _camera = GameObject.FindObjectOfType<SmoothCamera>();
            spriteRenderer = GetComponent<SpriteRenderer>();

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
            if (playerMovement != null)
            {
                if (playerMovement.isDashing)
                    Hit(_playerState.damages);
                else
                    _playerState.Hit(_damages);
            }
        }

        public void Hit(int damages)
        {
            health -= damages;
            if (health <= 0)
            {
                health = 0;
                _destroy();
            }
        }

        private void _destroy()
        {
            _playerState.Killed(swagBack);
            //TODO Hide object
            gameObject.GetComponent<Renderer>().enabled = false;
            //TODO Trigger particles
            //TODO Trigger sound
            //TODO Wiggle camera
            _camera.WiggleCamera(SmoothCamera.WiggleForce.Low);
            //TODO Wait to destroy object
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
    }
}
