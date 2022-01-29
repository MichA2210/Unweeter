using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float waitTime = 4f;

    private void Start()
    {
        Destroy(gameObject, waitTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
