using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{ 
    public Texture2D[] cursorImages;
    public static CrosshairManager Instance { get; private set; }

    private void Awake()
    {
        //This will only pass once at the beggining of the game 
        if (Instance == null){
            Instance = this;//Self reference
        }
        else{
            Destroy(gameObject);//Destroy duplicate instance
        }
    }

    public void SetCrosshairTo(int indexOfCrosshairChoice)
    {
        Vector2 hotSpot = Vector2.zero;
        if(cursorImages[indexOfCrosshairChoice].name != "Gauntlet"){
            hotSpot = new Vector2(cursorImages[indexOfCrosshairChoice].width / 2, cursorImages[indexOfCrosshairChoice].height / 2);
        }
        Cursor.SetCursor(cursorImages[indexOfCrosshairChoice], hotSpot, CursorMode.Auto);
    }

}
