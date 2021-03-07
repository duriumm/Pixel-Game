using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D myPlayerRB;
    //public Animator bootsAnimator;        // Use when equipment is being added
    public Animator animator;

    //public float bootsSpeedGang;          // Use when equipment is being added

    //public GameObject feetSocketTest;     // Use when equipment is being added

    public Vector2 movement;

    [SerializeField]
   
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        if(Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            animator.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
        }

        //bootsAnimator.SetFloat("Horizontal", movement.x);             // Use when equipment is being added
        //bootsAnimator.SetFloat("Vertical", movement.y);               // Use when equipment is being added

        animator.SetFloat("Speed", movement.sqrMagnitude);
        //bootsAnimator.SetFloat("Speed", movement.sqrMagnitude/* * bootsSpeedGang*/);      // Use when equipment is being added


    }

    private void FixedUpdate()
    {
        // movement
        myPlayerRB.MovePosition(myPlayerRB.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
