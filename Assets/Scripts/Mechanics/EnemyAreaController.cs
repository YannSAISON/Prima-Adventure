using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class EnemyAreaController : MonoBehaviour
{
    public float speed;
    public float chasingSpeed;

    public int damages;
    // public Vector2 add;
    
    private BasicEnemyController _enemyController;
    private EnemyArea _enemyArea;
    
    // Start is called before the first frame update
    void Start()
    {
        _enemyController = this.GetComponentInChildren<BasicEnemyController>();
        _enemyArea = this.GetComponentInChildren<EnemyArea>();
    }

    public void OnPlayerEnterArea()
    {
        _enemyController.OnPlayerEnterArea();
    }
    
    public void OnPlayerExitArea()
    {
        _enemyController.OnPlayerExitArea();
    }

    public void Init(out Vector2 originalPosition, out Vector2 destination, out float outSpeed, out float outChasingSpeed,
        out int outDamages)
    {
        _enemyArea.Init(out originalPosition, out destination);
        outSpeed = speed;
        outChasingSpeed = chasingSpeed;
        outDamages = damages;
    }
}
