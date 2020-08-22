using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMove : MonoBehaviour
{
    private Vector2 cameraChange;
    //private Vector3 playerChange;
    private CamMove cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CamMove>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraChange = gameObject.transform.position;
            cam.minPosition = cameraChange;
            cam.maxPosition = cameraChange;
            //other.transform.position += playerChange;
        }
    }
}
