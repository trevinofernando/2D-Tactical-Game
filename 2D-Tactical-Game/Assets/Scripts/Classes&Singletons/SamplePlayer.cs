using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePlayer
{
    public string playerName;
    public int playerIndex;
    public int playerHealth = 100;
    // player color (NEEDS TO CHANGE TO COLOR TYPE)
    public Color playerColor;
    public int playerTeam;

    // constructor
    public SamplePlayer (int playerNumber)
    {
        playerIndex = playerNumber;
        playerName = "Player " + playerNumber;
    }

}