using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Vector3 moveDirection;
    private PlayerStats stats;
    private Animator animator;
    private Rigidbody2D rigidbody;
    private SpriteRenderer sr;

    void Start() {
        stats = gameObject.GetComponent<PlayerStats>();
        animator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() {
        //only accept input while the game is running
        if(Global.gameStats == GameStats.RUNNING) {
            //Move
            moveDirection = new Vector3(0.0f, 0.0f, 0.0f);
            if (Input.GetKey(KeyCode.W)) moveDirection.y += 1.0f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1.0f;
            if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1.0f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x += 1.0f;

            transform.Translate(moveDirection * stats.speed);
            animator.SetBool("isWalking", moveDirection != new Vector3(0.0f, 0.0f, 0.0f));
            transform.position = new Vector3(transform.position.x, transform.position.y);
            sr.sortingOrder = -(int)Mathf.Round(transform.position.y - sr.bounds.size.y / 2);  //fix the y pos to the bottom

            //Facing
            if (moveDirection.x > 0.0f) transform.localScale = new Vector2(-1.0f, 1.0f);
            if (moveDirection.x < 0.0f) transform.localScale = new Vector2(1.0f, 1.0f);
        }
    }

}
