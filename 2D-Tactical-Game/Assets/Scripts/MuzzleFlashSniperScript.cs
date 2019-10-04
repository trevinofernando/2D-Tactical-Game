using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashSniperScript : MonoBehaviour
{
    public int damage = 50;
    public Color startColor = new Color(1, 1, 0, 1); //Yellow
    public Color endColor = new Color(1, 1, 1, 1); //White
    public Material mat;
    public float traceDuration = 0.01f;
    private RaycastHit2D hit;
    private DamageHandler dh;
    private Vector2 origin;
    private Vector2 direction;
    private Vector3 dir3;

    void Start()
    {

        Vector3 dir3 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir3.Normalize();
        dir3.z = 0;

        origin = transform.position;
        direction = dir3;
        hit = Physics2D.Raycast(origin, direction);
        if(hit.transform != null){
            DrawLine(transform.position, hit.transform.position, startColor, endColor, traceDuration);
            dh = hit.transform.GetComponent<DamageHandler>();
            if (dh != null)
            {
                dh.TakeDamage(damage);
            }
        }
        else
        {
            DrawLine(transform.position, dir3 * 1000f, startColor, endColor, traceDuration);
        }
        Destroy(gameObject, traceDuration);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color1, Color color2, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = mat;
        lr.startColor = color1;
        lr.endColor = color2;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

}
