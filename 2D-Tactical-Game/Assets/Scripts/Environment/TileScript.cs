using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public LayerMask layerMask;
    private Collider2D coll;
    private Color red = new Color(1f, 0f, 0f, 1f);
    public bool amIOnMap = false;


    void Awake()
    {
        coll = gameObject.GetComponent<Collider2D>();
    }

    void Start()
    {
        Vector2 spawnPoint = transform.position;
        
        Collider2D overlap = Physics2D.OverlapBox(spawnPoint, new Vector2(0.99f, 0.99f), layerMask);

        if(overlap!= null && overlap.transform.tag == "Ground")
        {
            SpriteRenderer sr = overlap.transform.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                sr.color = red;
            }

            TileScript ts = overlap.transform.GetComponent<TileScript>();

            if(ts != null)
            {
                Debug.Log("There's a tile script!");
                Debug.Log("On map? "+ts.amIOnMap + " "+ transform.position);
            }
            else
            {
                Debug.Log("No tile script");
            }

            Debug.Log(overlap.tag + " "+overlap.transform.position);
            Destroy(gameObject);
        }

        amIOnMap = true;
        coll.enabled = true;
    }
}
