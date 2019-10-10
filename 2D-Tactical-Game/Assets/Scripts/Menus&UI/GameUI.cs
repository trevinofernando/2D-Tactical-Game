using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public GameManager GM;
    public TextMeshProUGUI turnTimer;
    public TextMeshProUGUI gameTimer;

    // Start is called before the first frame update
    void Start()
    {
        // pull initial values into UI elements
        turnTimer.text = GM.turnClock.ToString();
        gameTimer.text = ConvertToMinutesAndSeconds(GM.gameClock);
    }

    // Update is called once per frame
    void Update()
    {
        // update UI elements
        turnTimer.text = GM.turnClock.ToString();
        gameTimer.text = ConvertToMinutesAndSeconds(GM.gameClock);
    }

    public string ConvertToMinutesAndSeconds(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        return (minutes.ToString("00") + ":" + seconds.ToString("00"));
    }
}
