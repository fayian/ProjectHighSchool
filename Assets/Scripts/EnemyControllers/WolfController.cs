using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfController : EnemyController {
    private CapsuleCollider2D col;

    private bool charging = false;
    private bool dealtDamage = false;

    private IEnumerator Charge(float distance, float chargeSpeed = 0.25f) {
        charging = true;
        dealtDamage = false;
        yield return new WaitForSeconds(0.4f); //0.4 sec delay
        Vector2 direction = (Global.player.transform.position - transform.position).normalized;
        Vector2 chargeEnd = (Vector2)transform.position + direction * distance;
        yield return new WaitForSeconds(0.3f); //0.3 sec to dodge after aiming
        RaycastHit2D[] hits;
        while (Vector2.Distance(transform.position, chargeEnd) > Mathf.Epsilon) {
            int divide = 20; //divide the distance into n parts to prevent passing through walls
            for (int i = 0; i < divide; i++) {
                hits = Physics2D.CapsuleCastAll((Vector2)transform.position + col.offset, col.bounds.size, col.direction, 0.0f, direction, chargeSpeed / divide); 
                if (hits.Any(hit => hit.collider.gameObject.tag == "Wall")) { //collided with a wall
                    charging = false;
                    yield break;
                }

                transform.position = Vector2.MoveTowards(transform.position, chargeEnd, chargeSpeed / divide);                
                if (!dealtDamage && col.IsTouching(Global.player.GetComponent<CapsuleCollider2D>())) {
                    //if this charge hits the player, deal damage to player
                    dealtDamage = true;
                    Global.player.GetComponent<PlayerStats>().DealDamage(GetComponent<EnemyStats>().damage);
                }
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
            yield return null;
        }

        charging = false;
    }
    void Start() {
        col = GetComponent<CapsuleCollider2D>();
        SearchPath();
    }

    void FixedUpdate() {
        if (Global.gameState == GameState.RUNNING) {
            if (Vector2.Distance(Global.player.transform.position, transform.position) > 1.5f && !charging) {
                MoveTowardPlayer();
            } else if (!charging) {
                StartCoroutine(Charge(2.5f));
            }
        }
        
    }

}
