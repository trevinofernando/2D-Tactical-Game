using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprialMovement : MonoBehaviour
{

    public int maxCycles = 2;
    public float Speed = 10f;
    public float GrowSpeed = 0.1f;
    public float maxCircleSize = 30f;

    private Vector2 center = Vector2.zero;
    private float circleSize = 0f;
    private float x;
    private float y;
    private int counter = 1;

    void Start()
    {
        center = transform.position;
        counter = 1;
    }


    void FixedUpdate()
    {
        if(circleSize > maxCircleSize){
            circleSize = 0f;
            counter++;
        }
        if(counter > maxCycles){
            Destroy(gameObject);
        }
        x = Mathf.Sin(Time.time * Speed) * circleSize;
        y = Mathf.Cos(Time.time * Speed) * circleSize;
        circleSize += GrowSpeed;
        transform.position = new Vector2(center.x + x, center.y + y);
    }
}
