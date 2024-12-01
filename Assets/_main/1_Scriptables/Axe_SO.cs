using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Parameters/Axe")]
public class Axe_SO : ScriptableObject
{
    public int upgrade;
    public int valueToUpgrade;
    public float secToCut;
    public int logsMax;
    public Sprite icon;
}
