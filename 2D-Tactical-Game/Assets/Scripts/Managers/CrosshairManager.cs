using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{ 
    public GameObject TileSpawnerManager;
    public SpriteRenderer TilePrefab;
    public Sprite[] tileSprites;
    public Texture2D[] cursorImages;
    public static CrosshairManager Instance { get; private set; }

    private int numTiles;
    private Vector2 hotSpot;

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

    private void Start() {
        if(TileSpawnerManager == null){
            Debug.LogError("Missing reference to TileSpawnerManager");
        }
        numTiles = tileSprites.Length;
    }

    public void SetCrosshairTo(int indexOfCrosshairChoice)
    {
        if(indexOfCrosshairChoice >= (int)WeaponCodes.Weak_Stone && indexOfCrosshairChoice < (int)WeaponCodes.Weak_Stone + numTiles){
            //Enable child to show tile and follow mouse
            TileSpawnerManager.SetActive(true);
            //Disabel Mouse
            Cursor.visible = false;
            //Set the GrandChild to the corresponding sprite by substracting the weakest tile from the choice
            TilePrefab.sprite = tileSprites[indexOfCrosshairChoice - (int)WeaponCodes.Weak_Stone];
        }else{
            //Disable Tile spawner
            TileSpawnerManager.SetActive(false);
            //Enable Cursor
            Cursor.visible = true;

            //Set offset to cursor to be the center unless its the Gauntlet
            if(cursorImages[indexOfCrosshairChoice].name == "Gauntlet"){
                hotSpot = Vector2.zero;
            }else{
                hotSpot = new Vector2(cursorImages[indexOfCrosshairChoice].width / 2, cursorImages[indexOfCrosshairChoice].height / 2);
            }
            Cursor.SetCursor(cursorImages[indexOfCrosshairChoice], hotSpot, CursorMode.Auto);
        }
    }

}
