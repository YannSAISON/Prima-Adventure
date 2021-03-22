using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public int health = 100;
    public int damages = 50;

    private SmoothCamera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindObjectOfType<SmoothCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(int damages)
    {
        health -= damages;
        if (health < 0)
        {
            _camera.WiggleCamera(SmoothCamera.WiggleForce.High);
            Debug.Log("Dead.");
        }
    }
}
