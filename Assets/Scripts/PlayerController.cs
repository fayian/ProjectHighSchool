using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//there are two colliders on the player
//the circle one is for physics
//and the capsule one is for damage judgement
public class PlayerController : MonoBehaviour {

    private Vector3 moveDirection;
    private PlayerStats stats;
    private Animator animator;
    private SpriteRenderer sr;

    void Start() {
        Global.player = gameObject;        
        stats = gameObject.GetComponent<PlayerStats>();
        animator = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() {
        //only accept input while the game is running
        if(Global.gameState == GameState.RUNNING) {
            //Move
            moveDirection = new Vector3(0.0f, 0.0f, 0.0f);
            if (Input.GetKey(KeyCode.W)) moveDirection.y += 1.0f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1.0f;
            if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1.0f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x += 1.0f;

            transform.Translate(moveDirection * stats.speed);
            animator.SetBool("isWalking", moveDirection != new Vector3(0.0f, 0.0f, 0.0f));
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f/*slightly higher if in same tile with obstacles*/);
            sr.sortingOrder = -(int)Mathf.Round(transform.position.y);  //set sorting order

            //Facing
            if (Input.mousePosition.x > Camera.main.pixelWidth / 2) transform.localScale = new Vector2(-1.0f, 1.0f);
            else transform.localScale = new Vector2(1.0f, 1.0f);
        }
    }

}
