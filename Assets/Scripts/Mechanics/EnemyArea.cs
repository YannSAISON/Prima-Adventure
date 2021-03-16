using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    private EnemyAreaController _enemyAreaController;
    
    private void Start()
    {
        _enemyAreaController = this.GetComponentInParent<EnemyAreaController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered !");
            _enemyAreaController.OnPlayerEnterArea();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited !");
            _enemyAreaController.OnPlayerExitArea();
        }
    }

    public void Init(out Vector2 startPosition, out Vector2 endPosition)
    {
        // GameObject.FindObjectsOfType();
        Transform[] start = gameObject.GetComponentsInChildren<Transform>();
        BoxCollider2D tmpCollider = this.GetComponent<BoxCollider2D>();
        startPosition = start[1].position;
        endPosition = start[2].position;
        Debug.Log("Size of start: " + start.Length.ToString());
        // Bounds bounds = tmpCollider.bounds;
        // var xHalfExtents = bounds.extents.x;
        // var yHalfExtents = bounds.extents.y;
        // var xCenter = bounds.center.x;
        // var yCenter = bounds.center.y;
        // // float xLeft = xCenter - xHalfExtents;
        // // float xRight = xCenter + xHalfExtents;
        // startPosition = new Vector2(xCenter - xHalfExtents, yCenter + yHalfExtents);
        // endPosition = new Vector2(xCenter + xHalfExtents, yCenter + yHalfExtents);
    }
}
