using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game State", menuName = "State/Game")]
public class GameState_SO : ScriptableObject
{
    [Header("Parameters")]
    public float coldDamageTimer;
    public float timeToCold;
    public Axe_SO currentAxe;
    public Bow_SO currentBow;

    [Header("Dynamic stats")]
    public bool shouldTakeColdDamage;
    public float coldDamageTimerLeft;
    public float playTime;
    public bool didGoHunt;
    public bool didGoCut;
    public int playerHealth;
    public bool didBurnWood;
    public bool didCook;
    public bool woodTutorShown;
    public bool getBackTutorialShown;
    public bool noArrowsTutorialShown;
    public bool coldTutorialShown;
    public bool meetTutorialShown;
    public bool rulesTutorialShown;
    public int level;
    public int temperature;
    public int arrows;
    public int meatToCook;
    public int woodsToBurn;
    public int meat, wood, teaPlants, tea;
    public int meatInThisRun, woodInThisRun, teaPlantsThisRun;
    public GameState current;

    public bool didShowFireCaution;
    public bool didShowFoodCaution;
}


public enum GameState
{
    Lobby,
    Hunt,
    Wood
}
