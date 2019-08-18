using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour {
    public float damage;
    public string targetTag;

    const float duration = 0.2f;
    private float timer = 0.0f;

    private BoxCollider2D col;
    private HashSet<GameObject> alreadyAttacked = new HashSet<GameObject>();
    private List<Collider2D> collisions = new List<Collider2D>();

    void Start() {
        col = GetComponent<BoxCollider2D>();
    }

    void Update() {
        if (timer > duration) Destroy(gameObject);
        timer += Time.deltaTime;

        col.GetContacts(collisions);
        var targets = from collision in collisions
                               where collision.gameObject.tag == targetTag
                               select collision.gameObject;
        foreach(GameObject obj in targets) {
            if (alreadyAttacked.Contains(obj)) continue;
            else {
                alreadyAttacked.Add(obj);
                obj.GetComponent<Stats>().DealDamage(damage);
            }
        }
    }
}
