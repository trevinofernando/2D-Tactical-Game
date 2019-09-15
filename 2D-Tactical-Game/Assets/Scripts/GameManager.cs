using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GlobalVariables GLOBALS;
    public GameObject avatarPrefab;
    public Vector3 spawnOffset = new Vector3(-18, 10, 0);

    void Start()
    {
        GLOBALS = GlobalVariables.Instance;
        GLOBALS.gameState = GlobalVariables.GameState.LoadingScene;

        //Create Teams
        for (int i = 0; i < GLOBALS.numTeams; i++)
        {
            for(int j = 0; j < GLOBALS.teamSize; j++)
            {
                spawnOffset.x += 2;
                GLOBALS.teams[i , j] = Instantiate(avatarPrefab, transform.position + spawnOffset, transform.rotation);
                GLOBALS.teams[i , j].GetComponent<PlayerSettings>().SetColor(GLOBALS.teamColors[i]);
            }
        }
    }


    void Update()
    {
        switch(GLOBALS.gameState)
        {
            case GlobalVariables.GameState.LoadingScene:
                
                break;

            case GlobalVariables.GameState.MovingAvatar:
                
                break;

            case GlobalVariables.GameState.Pause:

                break;

            case GlobalVariables.GameState.Shooting:

                break;

            case GlobalVariables.GameState.TurnTransition:

                break;

            case GlobalVariables.GameState.WeaponSelect:

                break;

            default:
                Debug.LogError("Invalid State reached... but HOW??!!");
                break;
        }
    }
}
