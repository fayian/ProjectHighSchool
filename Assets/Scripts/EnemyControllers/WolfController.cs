using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfController : EnemyController {
    private SpriteRenderer sr;
    private Collider2D col;

    private bool charging = false;
    private int collisionCount = 0;
    private bool dealtDamage = false;
      
    private IEnumerator Charge(float distance) {
        charging = true;
        dealtDamage = false;
        yield return new WaitForSeconds(0.4f); //0.4 sec delay
        Vector2 direction = (Global.player.transform.position - transform.position).normalized;
        Vector2 chargeEnd = (Vector2)transform.position + direction * distance;
        yield return new WaitForSeconds(0.3f); //0.3 sec to dodge after aiming

        int beginCollisionCount = collisionCount;
        while (Vector2.Distance(transform.position, chargeEnd) > Mathf.Epsilon) {
            int divide = 10; //divide the distance into n parts to prevent passing through walls
            for (int i = 0; i < divide; i++) {
                transform.position = Vector2.MoveTowards(transform.position, chargeEnd, 0.25f / divide);

                if(!dealtDamage && col.IsTouching(Global.player.GetComponent<CapsuleCollider2D>())) {
                    //if this charge hits the player, deal damage to player
                    dealtDamage = true;
                    Global.player.GetComponent<PlayerStats>().currentHealth -= GetComponent<EnemyStats>().damage;
                }
                if (collisionCount - beginCollisionCount > 0) { //collide with a wall after start charging
                    //end the charge
                    charging = false;
                    yield break;
                }
            }                
            yield return null;
        }

        charging = false;
    }
    void Start() {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        SearchPath();
    }

    void FixedUpdate() {
        sr.sortingOrder = -(int)transform.position.y;
        if (Global.gameState == GameState.RUNNING) {
            if (Vector2.Distance(Global.player.transform.position, transform.position) > 1.5f && !charging) {
                MoveTowardPlayer();
            } else if (!charging) {
                StartCoroutine(Charge(2.5f));
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Wall") collisionCount++;
    }
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Wall") collisionCount--;
    }
}
