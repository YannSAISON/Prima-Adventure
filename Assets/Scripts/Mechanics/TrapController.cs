using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public int damages;
    public float timeBetweenHits;
    private float _timeRemaining;

    private PlayerStateManager _playerState;

    // Start is called before the first frame update
    void Start()
    {
        _playerState = GameObject.Find("_Player").GetComponent<PlayerStateManager>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
        if (!other.CompareTag("Player") || playerMovement == null || !playerMovement.isEnabled)
            return;
        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0)
        {
            _timeRemaining = timeBetweenHits;
            _playerState.Hit(damages);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
        if (playerMovement != null && playerMovement.isEnabled)
        {
            _timeRemaining = timeBetweenHits;
            _playerState.Hit(damages);
        }
    }
}
