using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
    public float maxHealth;
    public float currentHealth;
    public float speed;
    public virtual void DealDamage(float damage) {
        currentHealth -= damage;
    }
}
