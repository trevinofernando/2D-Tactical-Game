using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform gameManager; //Self reference

    public int numPlayersHuman = 4; 
    public int numPlayersNPCs = 0;
    public int squadSizePerTeam = 4;

    public float timePerTurn = 60f; //1 minute
    public float timeBetweenTurns = 5f; // 5 seconds
    public float TimePerGame = 60f * 20f; // 20 minutes

    struct Employee
    {
        public int EmpId;
        public string FirstName;
        public string LastName;

        public Employee(int empid, string fname, string lname)
        {
            EmpId = empid;
            FirstName = fname;
            LastName = lname;
        }
    }

    void Start()
    {
        //GameObject go = Instantiate(A, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //go.transform.parent = GameObject.Find("Stage Scroll").transform;

        //float clock = GlobalVariables.Instance.TimePerGame;
    }


    void Update()
    {
        
    }
}
