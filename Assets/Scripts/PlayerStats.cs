using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class PlayerStats : MonoBehaviour {
    public float maxHealth = 100.0f;
    public float currentHealth = 100.0f;
    public float maxMana = 100.0f;
    public float currentMana = 100.0f;
    public float maxStamina = 100.0f;
    public float currentStamina = 100.0f;

    public float speed = 0.5f;
    public float dodgeRate = 0.05f;
}
