using UnityEngine;

//Singleton implementation for global variables

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    //      GLOBAL VARIABLES         //

    //Teams related variables
    public int numPlayersHuman = 4;
    public int numPlayersNPCs = 0;
    public int squadSizePerTeam = 4;
    public string[] teamNames;

    //Time related variables
    public float timePerTurn = 60f; //1 minute
    public float timeBetweenTurns = 5f; // 5 seconds
    public float TimePerGame = 60f * 20f; // 20 minutes

    //

    private void Awake()
    {
        //This will only pass once at the beggining of the game 
        if (Instance == null)
        {
            //Self reference
            Instance = this;
            //Make this object persistent
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy duplicate instance created by changing Scene
            Destroy(gameObject);
        }
    }
}
