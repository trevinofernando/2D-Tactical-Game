using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{ 
    public Sprite[] sprites;

    private Vector3 mousePosition;
    private Vector2 offset;
    private Image img;
    private bool onSyncWithFixedUpdate;

    void Awake()
    {
        img = gameObject.GetComponent<Image>();
    }

    void Start()
    {
        //Hide mouse cursor to show crosshair instead
        Cursor.visible = false;
        img.sprite = sprites[0];
    }

    void LateUpdate()
    {
        //Sync with Fixed update to stop weird cursor movement when following plane
        if(!onSyncWithFixedUpdate && Time.timeScale != 0.0f){ //Ignore sync if time is stopped
            return;
        }
        else{
            onSyncWithFixedUpdate = false;
        }
        //Find Mouse position in monitor and then translate that to a point in the world
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        //sprite[0] is the gauntlet
        if(img.sprite == sprites[0]){
            //offset the sprite to have the gauntlet finger tip exactly on the cursor tip
            offset = new Vector2(-0.45f, 0.7f); 
        }else{
            offset = Vector2.zero;
        }
        // Move Crosshair to mouse position. And leave the z coordinate alone
        transform.position = (new Vector2(mousePosition.x, mousePosition.y)) - offset;
    }

    public void SetCrosshairTo(int indexOfCrosshairChoice)
    {
        img.sprite = sprites[indexOfCrosshairChoice];
        Cursor.visible = false; //just in case the cursor was enable by other method
    }

    private void FixedUpdate() {
        onSyncWithFixedUpdate = true;
    }
}
