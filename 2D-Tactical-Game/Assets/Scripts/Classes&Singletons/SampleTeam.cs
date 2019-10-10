using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for team setup, plan on instantiating it for each team
public class SampleTeam
{
    public string teamName;
    public int teamNumber;
    public Color teamColor = new Color();
    public int teamColorIndex;
    public bool isAI = false;
    public SamplePlayer[] roster = new SamplePlayer[8];



    // constructor on making a new team
    public SampleTeam (int numPlayers)
    {
        // fill up the roster with players
        for (int i = 0; i < numPlayers; i++)
        {
            // make each player
            roster[i] = new SamplePlayer(i);
        }
    }

}