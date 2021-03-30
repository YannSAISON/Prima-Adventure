using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 = need K door
    // 2 = need L door
    // 3 = need M door
    // 4 = need Q door
    // 5 = need R door
    // 6 = need S door

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
        if (templates.nbRooms == 0)
        {
            Instantiate(templates.BossRooms[openingDirection - 1], transform.position, Quaternion.identity);
            spawned = true;
        }
        if (spawned == false)
        {
            switch (openingDirection)
            {
                case 1:
                    rand = UnityEngine.Random.Range(0, templates.KRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.KRooms[rand], transform.position, Quaternion.identity);
                    break;
                case 2:
                    rand = UnityEngine.Random.Range(0, templates.LRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.LRooms[rand], transform.position, Quaternion.identity);
                    break;
                case 3:
                    rand = UnityEngine.Random.Range(0, templates.MRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.MRooms[rand], transform.position, Quaternion.identity);
                    break;
                case 4:
                    rand = UnityEngine.Random.Range(0, templates.QRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.QRooms[rand], transform.position, Quaternion.identity);
                    break;
                case 5:
                    rand = UnityEngine.Random.Range(0, templates.RRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.RRooms[rand], transform.position, Quaternion.identity);
                    break;
                case 6:
                    rand = UnityEngine.Random.Range(0, templates.SRooms.Length);
                    Debug.Log("Spawning something at " + transform.position + "\n");
                    Instantiate(templates.SRooms[rand], transform.position, Quaternion.identity);
                    break;
            }
            spawned = true;
            templates.nbRooms--;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(templates.Boss, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
