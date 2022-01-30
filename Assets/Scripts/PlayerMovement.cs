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
    [SerializeField]
    bool canMove = true;

    private Animator animator;

    // Update is called once per frame
    private void Start()
    {
        enemyRoom = null;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if(canMove)
        {
            animator.SetFloat("Moving", Mathf.Abs(horizontalMove));
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }

            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
            }
        }

        if (enemyRoom == null)
        {
            SetMove(true);
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
            controller.Move( ( canMove ? horizontalMove : 0) * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }

    public void SetMove(bool move)
    {
        canMove = move;
    }

    GameObject enemyRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.gameObject.layer == 10 && enemyRoom == null )
        {
            enemyRoom = other.gameObject;
            TBPlayer player = GetComponent<TBPlayer>();
            TBEnemy enemy = other.GetComponentInChildren<TBEnemy>();
            SetMove(false);
            CombatSystem = new TBCombat( player, enemy);
            SetMove(false);
        }
    }
}
