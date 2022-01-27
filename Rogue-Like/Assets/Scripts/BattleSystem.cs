using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject player;
    public GameObject battleStand;
    public GameObject enemy;

    public Transform playerPos;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            state = BattleState.START;

            other.transform.position = playerPos.position;
            player = other.gameObject;
            player.SetActive(false);
            SetupBattle();
        }
    }
    
    void SetupBattle()
    {
        Instantiate(battleStand, playerPos);
    }

}
