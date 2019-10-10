using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITarget : MonoBehaviour
{

    public static List<GameObject> DecideTargets(
        Vector3 currentPosition, int numTeams, int numPlayers, int myTeamId, GameObject[,] teams, bool sortDecreasing)
    {
        List<GameObject> targets = new List<GameObject>();

        for (int i = 0; i < numTeams; i++)
        {
            if (i != myTeamId )
            {
                for (int j = 0; j < numPlayers; j++)
                {
                    if (teams[i, j] != null)
                    {
                        if (IsShootable(currentPosition, teams[i, j].transform.position))
                        {
                            targets.Add(teams[i, j]);
                        }

                    }
                }
            }

        }


        //TODO sort after writing player comparison by health
        //targets.Sort((t1, t2) => );

        return targets;
    }

    //Checks if you can ray cast to a player
    public static bool IsShootable(Vector3 currentPosition, Vector3 otherPosition)
    {
        RaycastHit2D raycast = Physics2D.Raycast(currentPosition, (otherPosition - currentPosition).normalized);

        return raycast.transform.position == otherPosition;
    }
}
