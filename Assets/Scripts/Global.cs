using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStats { RUNNING, PAUSE }
public static class Global {
    public static GameStats gameStats = GameStats.RUNNING;
}
