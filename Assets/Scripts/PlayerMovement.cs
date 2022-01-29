using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public TBCombat CombatSystem = null;
    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    bool canMove = true;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * ( canMove ? 1f : 0f );
       
        if (Input.GetButtonDown("Jump") && canMove)
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch") && canMove)
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch") && canMove)
        {
            crouch = false;
        }

        if(Input.GetButtonDown("Select") && canMove)
        {
            GetComponent<Animator>().SetTrigger("Move");
        }
    }

    private void FixedUpdate()
    {
        if (crouch == true)
        {
            horizontalMove = 0;
        }
        else
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.gameObject.layer == 10 )
        {
            TBPlayer player = GetComponent<TBPlayer>();
            TBEnemy enemy = other.GetComponentInChildren<TBEnemy>();
            CombatSystem = new TBCombat( player, enemy);
            SetMove(false);
        }
    }

    private void SetMove(bool move)
    {
        canMove = move;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            SetMove(true);
        }
    }
}
