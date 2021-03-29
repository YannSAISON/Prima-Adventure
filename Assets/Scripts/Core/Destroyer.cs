using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trying to destroy " + other.GetInstanceID());
        Destroy(other.gameObject);
    }
}
