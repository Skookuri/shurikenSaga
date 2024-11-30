using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    // Abilities that can be unlocked
    public static bool canShift = false; //only after Dungeon1Intro
    public static bool canShurithrow = false; //only after Dungeon1Intro
    public static bool canDash = false; //only after Dungeon1

    void Start()
    {
        canShift = false;
        canShurithrow = false;
        canDash = false;
    }

    void Update()
    {
        // if (true.turnOn) 
    }
}