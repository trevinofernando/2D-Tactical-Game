using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamAndPlayerPrototype : MonoBehaviour
{
    // okay
    // idea
    // so how about, on game startup,
    // you initialize an array of 8 teams
    // with each team initializing 8 players
}

/*

    
    // i think these need to be in another script vvvv

    public string newPlayer (int playerNumber)
    {
        // WIP based on how to determine in game how many players are on a team
        // (i.e. +1 -1 system or 1 -> 8 slider/field)
    }

    // when the AI box is ticked, the box will call this method dynamically via script
    public void checkForAIBox ()
    {
        // if the checkbox is ON, set team.isAI = true
        // if the checkbox is OFF, set team.isAI = false
    }

    public void changeName (String playerOrTeam)
    {
        // tie this method to the field box
        // every time its updated update the string
        // might be taxing based on how strings work in C#

    }

    public void changeColor ()
    {
        // thie this to the color dropdown / picker
        // every time color is changed update the color var

    }
}



*/