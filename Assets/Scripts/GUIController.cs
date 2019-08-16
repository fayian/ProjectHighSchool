using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {
    public GameObject HPBar;
    public GameObject MPBar;
    public GameObject SPBar;

    private PlayerStats stats;
    private Slider HPBarSlider;
    private Slider MPBarSlider;
    private Slider SPBarSlider;
    
    void Start() {
        stats = Global.player.GetComponent<PlayerStats>();
        HPBarSlider = HPBar.GetComponent<Slider>();
        MPBarSlider = MPBar.GetComponent<Slider>();
        SPBarSlider = SPBar.GetComponent<Slider>();
    }

    void Update() {
        HPBarSlider.maxValue = stats.maxHealth;
        HPBarSlider.value = stats.currentHealth;
        MPBarSlider.maxValue = stats.maxMana;
        MPBarSlider.value = stats.currentMana;
        SPBarSlider.maxValue = stats.maxStamina;
        SPBarSlider.value = stats.currentStamina;
    }
}
