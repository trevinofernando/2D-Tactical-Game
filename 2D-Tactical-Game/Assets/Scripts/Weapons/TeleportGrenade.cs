using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGrenade : MonoBehaviour
{
    public float delay = 2f;
    public int timer = 5;
    public float launchForce = 1000f;
    public GameObject impactEffect;
    public string spawnSoundName = "Grenade_Launcher";
    public string explosionSoundName = "Dark_Explosion";
    public string impactSound1 = "Metal_Hit_High";
    public string impactSound2 = "Metal_Hit_Low";
    public float autoDestroyOnHeight = -50f;
    private bool soundAlternator;
    private GameManager gm;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private TrailRenderer tr;
    private GameObject soldier;
    private Vector3 location;
    private bool isInvisible = false;



    void Start()
    {
        gm = GlobalVariables.Instance.GM;
        soldier = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();

        AudioManager.instance.Play(spawnSoundName);
        
        //Add initial force once to make a parabolic trajectory
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * launchForce * rb.mass);
        if (rb == null)
            Debug.Log("null rigidbody");

        StartCoroutine(Timer((float)timer));
    }

    void FixedUpdate()
    {
        if(gameObject.transform.position.y <= autoDestroyOnHeight)
        {
            Destroy(gameObject);
        }

        //Calculate rotation amount given the horizontal speed
        rb.angularVelocity = -rb.velocity.x * 25f; 
    }

    void Detonate()
    {
        //Explosion Sound
        AudioManager.instance.Play(explosionSoundName);

        // grab location of grenade
        location = gameObject.transform.position;

        // make grenade dissapear
        isInvisible = true;
        tr.enabled = false;
        sr.enabled = false;

        // set focus to the player
        CameraController.instance.soldier = soldier;

        Destroy(gameObject, (8 * delay));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(isInvisible){
            return;
        }
        if(soundAlternator){
            AudioManager.instance.Play(impactSound1);
        }else{
            AudioManager.instance.Play(impactSound2);
        }
        soundAlternator = !soundAlternator;
    }

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime - delay);
        if(delay > 0){
            Detonate();
            // anim 1
            if (impactEffect != null)
            {
                yield return new WaitForSeconds(delay);
                Instantiate(impactEffect, soldier.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.25f); // need to figure out frames here
                soldier.transform.position = new Vector3(soldier.transform.position.x, soldier.transform.position.y, -100); // make him "dissapear!"
            }
            yield return new WaitForSeconds(delay - 0.25f);     // minus whatever frames you need above
        }
        rb = soldier.GetComponent<Rigidbody2D>();
        soldier.transform.position = new Vector3(location.x, location.y, -100); // set pos to "invisible" new position
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        // anim 2
        if (impactEffect != null)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(impactEffect, location, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
            soldier.transform.position = location;
        }
        yield return new WaitForSeconds(delay - 0.25f);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    } 
    private IEnumerator Timer2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
