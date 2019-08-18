using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class EnemyStats : Stats {
    public float damage;

    public override void DealDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
