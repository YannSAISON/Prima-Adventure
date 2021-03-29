using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public int _damages;

    private PlayerStateManager _playerState;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerState = GameObject.Find("_Player").GetComponent<PlayerStateManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovements playerMovement = other.gameObject.GetComponent<PlayerMovements>();
        if (playerMovement != null)
        {
            _playerState.Hit(_damages);
        }
    }
}
