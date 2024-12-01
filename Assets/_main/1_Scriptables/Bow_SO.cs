using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NEw Item", menuName = "Parameters/Bow")]
public class Bow_SO : ScriptableObject
{
    public int upgrade;
    public int valueToUpgrade;
    public int damage;
    public int meatMax;
    public Sprite icon;
}
