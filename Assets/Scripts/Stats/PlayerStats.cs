using UnityEngine;

internal class PlayerStats : Stats {
    public float healthRegen = 0.0f;

    public float maxMana = 100.0f;
    public float currentMana = 100.0f;
    public float manaRegen = 1.5f;

    public float maxStamina = 100.0f;
    public float currentStamina = 100.0f;
    public float staminaRegen = 1.5f;

    public float attackSpeed = 1.0f;
    private float attackTimer = 0.0f;
    //public float dodgeRate = 0.05f;

    public override void DealDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) Global.gameState = GameState.PAUSE;
    }

    public bool CanAttack() {  return attackTimer == 0.0f;  }
    public void Attack() { attackTimer = attackSpeed;  }


    private void Regeneration() {
        if(Global.gameState == GameState.RUNNING) {
            if (currentHealth < maxHealth) currentHealth = Mathf.Min(currentHealth + healthRegen, maxHealth);
            if (currentMana < maxMana) currentMana = Mathf.Min(currentMana + manaRegen, maxMana);
            if (currentStamina < maxStamina) currentStamina = Mathf.Min(currentStamina + staminaRegen, maxStamina);
        }        
    }

    void Start() {
        InvokeRepeating("Regeneration", 0.0f, 1.0f);
    }

    void Update() {
        if (attackTimer >= 0) attackTimer = Mathf.Max(0.0f, attackTimer - Time.deltaTime);
    }
}
