using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Prey", menuName = "Stats/Prey")]
public class Prey_SO : ScriptableObject
{
    public int meat;
    public int hp;
    public float speed;
    public float runSpeed;
}

public enum PreyType
{
    Hare,
    Fox,
    Deer,
    Wolf,
    Bear
}