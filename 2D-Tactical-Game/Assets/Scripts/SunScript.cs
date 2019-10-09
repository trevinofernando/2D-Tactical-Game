using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public GameManager GM;
    public GameObject[,] teams; //[TeamID, Soldier ID]
    public float damageIncreaseMultiplier = 1.1f; //Every time the sun shoots, the damage is increased 
    public float waitTimeMultiplier = 0.9f;
    private float waitTime = 60f;
    private float timeLastAttacked = 0.0f;
    private GlobalVariables GLOBALS;
    private int numTeams;
    private int teamSize;


    // Start is called before the first frame update
    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        numTeams = GLOBALS.numTeams;
        teamSize = GLOBALS.teamSize;
        

        //Call update every second after one minute has passed
        InvokeRepeating("UpdateEverySecond", 0f, 1f);
    }

    // Update is called once per frame
    void UpdateEverySecond()
    {
        if(GM.turnClock - timeLastAttacked  > waitTime)
        {
            if(GM.gameState == GameManager.GameState.TurnTransition)
            {
                Attack();
            }
        }

        
    }

    void Attack()
    {
        List<GameObject> targets = AITarget.DecideTargets(transform.position, numTeams, teamSize, -1, teams, true);
        if(targets.Count > 0)
        {
            Shoot(targets[0].transform.position);
            timeLastAttacked = GM.turnClock;
            waitTime *= waitTimeMultiplier;
            damageIncreaseMultiplier *= damageIncreaseMultiplier;
        }
        else
        {
            Shoot(new Vector3(0, 0, 0));
        }
    }


    void Shoot(Vector3 targetPosition)
    {

    }
}
