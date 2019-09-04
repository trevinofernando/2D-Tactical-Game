using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{ 
    public Sprite[] sprites;

    private Vector3 mousePosition;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //Hide mouse cursor to show crosshair instead
        Cursor.visible = false;
        spriteRenderer.sprite = sprites[0];

    }

    void FixedUpdate()
    {
        //Find Mouse position in monitor and then translate that to a point in the world
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        // Move Crosshair to mouse position. And leave the z coordinate alone
        transform.position = new Vector2(mousePosition.x, mousePosition.y);
    }

    public void SetCrosshairTo(int indexOfCrosshairChoice)
    {
        spriteRenderer.sprite = sprites[indexOfCrosshairChoice];
    }
}
