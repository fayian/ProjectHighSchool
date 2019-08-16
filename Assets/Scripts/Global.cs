using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { RUNNING, PAUSE }
public static class Global {
    public static bool gameStateChanged = false;
    public static GameState gameState = GameState.RUNNING;
    public static List<List<bool>> levelMapForMob = new List<List<bool>>();
    public static GameObject player;
    //false => no obstacle   true => have obstacle
}
