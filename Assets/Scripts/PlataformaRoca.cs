using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaRoca : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float crouchTime;
    private bool overPlatform;

    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }
    void Update()
    {
        
        if (Input.GetButtonDown("Crouch"))
        {
            if (crouchTime <= 0 && overPlatform == true)
            {
                effector.rotationalOffset = 180f;
                crouchTime = 0f;
            }
            else
            {
                crouchTime -= Time.deltaTime;
            }
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            effector.rotationalOffset = 0f;
            crouchTime = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            overPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            overPlatform = false;
        }
    }
}
