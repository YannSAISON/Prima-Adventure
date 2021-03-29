﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 = need B door
    // 2 = need T door
    // 3 = need L door
    // 4 = nees right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    public float waitTime = 4f;

    void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    void Spawn()
    {
        if (spawned == false)
        {
            if (openingDirection == 1)
            {
                rand = UnityEngine.Random.Range(0, templates.bottomRooms.Length);
                Debug.Log("Spawning something at " + transform.position + "\n");
                Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
            }
            else if (openingDirection == 2)
            {
                rand = UnityEngine.Random.Range(0, templates.topRooms.Length);
                Debug.Log("Spawning something at " + transform.position + "\n");
                Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);
            }
            else if (openingDirection == 3)
            {
                rand = UnityEngine.Random.Range(0, templates.leftRooms.Length);
                Debug.Log("Spawning something at " + transform.position + "\n");
                Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);
            }
            else if (openingDirection == 4)
            {
                rand = UnityEngine.Random.Range(0, templates.rightRooms.Length);
                Debug.Log("Spawning something at " + transform.position + "\n");
                Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);
            }
            spawned = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
