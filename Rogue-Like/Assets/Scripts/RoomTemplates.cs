using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;

    public List<GameObject> rooms;

    public float waitTime;
    private bool spawnedBoss;
    public GameObject boss;
    private bool spawnedMandatoryStatue;
    public GameObject mandatoryStatue;

    public GameObject[] entities;
    public bool fillRooms;
    private int rand;

    void Update()
    {
        if(waitTime <= 0 && spawnedBoss == false && spawnedMandatoryStatue == false && fillRooms == false)
        {
            for (int i=0; i < rooms.Count; i++)
            {
                if(i == rooms.Count - 1)
                {
                    Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
                    spawnedBoss = true;
                }
                else if (i == rooms.Count - 2)
                {
                    Instantiate(mandatoryStatue, rooms[i].transform.position, Quaternion.identity);
                    spawnedMandatoryStatue = true;
                    fillRooms = true;
                }
                else if (i < rooms.Count - 2 && i > 0)
                {
                    rand = Random.Range(0, entities.Length);
                    Instantiate(entities[rand], rooms[i].transform.position, Quaternion.identity);
                }
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }

}
