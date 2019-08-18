using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//there are two colliders on the player
//the circle one is for physics
//and the capsule one is for damage judgement
public class PlayerController : MonoBehaviour {
    private GameObject slashPrefab;

    private Vector3 moveDirection;
    private PlayerStats stats;
    private Animator animator;

    private readonly Vector2 SLASH_SIZE = new Vector2(0.5f, 0.2f);
    private const float SLASH_SPACE = 0.6f;
    private void CreateMeleeAttack(Vector2 size) {
        Vector2 facing = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        print(facing);
        GameObject slash = Instantiate(slashPrefab);
        AttackController attackController = slash.GetComponent<AttackController>();

        //set transform
        slash.transform.localScale = new Vector3(size.x / SLASH_SIZE.x, size.y / SLASH_SIZE.y, 1.0f);
        Vector2 pos = (Vector2)transform.position + facing * (size.y / 2 + SLASH_SPACE);
        slash.transform.position = new Vector3(pos.x, pos.y, pos.y);

        float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg - 90.0f;
        slash.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        //set script
        attackController.damage = 10.0f; //test data TODO: set the damage to weapon damage
        attackController.targetTag = "Enemy";
    }

    void Start() {
        Global.player = gameObject;
        slashPrefab = Resources.Load("Prefabs/SlashAttack") as GameObject;

        stats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
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
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

            //Facing
            if (Input.mousePosition.x > Camera.main.pixelWidth / 2) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            //Attack
            if(Input.GetMouseButtonDown(0) && stats.CanAttack()) {
                stats.Attack();
                animator.SetTrigger("Attack");
                CreateMeleeAttack(new Vector2(0.5f, 0.2f)); //test data TODO: get size from equipping weapon
            }
        }
    }

}
