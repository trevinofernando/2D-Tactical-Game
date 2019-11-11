using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGrenade : MonoBehaviour
{
    private GameManager gm;
    private CameraController cam;

    public int damageToPlayer = 60;
    public int damageToProps = 60;
    public int timer = 3;
    public bool IsHoly = false;
    public float launchForce = 1000f;
    public float explosionForce = 1000f;
    public float extraUpForce = -1f;
    public float explosionRadius = 4f;
    public GameObject impactEffect;
    public string spawnSoundName = "Grenade_Launcher";
    public string explosionSoundName = "Dark_Explosion";
    public string impactSound1 = "Metal_Hit_High";
    public string impactSound2 = "Metal_Hit_Low";
    public bool isTreeProjectile = false;
    public float autoDestroyOnHeight = -50f;
    private float rotationAmount;
    private bool soundAlternator;
    private Rigidbody2D rb;
    private CircleCollider2D cirColider;
    private DamageHandler target;

    void Start()
    {
        gm = GlobalVariables.Instance.GM;
        cam = GlobalVariables.Instance.GM.cam;

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

        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        /* steps:
         * shoot the thingy
         * when it detonates, make an implosion
         * swap camera over to the palyer
         * watch him dissapear into the void
         * swap camera BACK to implosion site
         * watch player reappear from the void
         */
        /*  TODO: MAKE THE CAMERA WORK AS INTENDED!!!!!!!!!!!!!!!!!!!!!!!
            // set focus to the player
            gm.cam.soldier = gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]];
            gm.cam.shouldFollowTarget = true;

            // wait a second
            Timer2(1);

            // set focus BACK to projectile
            gm.cam.soldier = gm.projectile;
            gm.cam.shouldFollowTarget = true;
        */
        // move player to the detonation point
        gm.teams[gm.currTeamTurn, gm.currSoldierTurn[gm.currTeamTurn]].transform.position = gameObject.transform.position;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {

        if(isTreeProjectile)
        {
            if (other.transform.GetComponent<TreeScript>())
            {
                Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<Collider2D>());
            }
                
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
        float delay = 0f;
        if(IsHoly){
            delay = 2f;
        }
        yield return new WaitForSeconds(waitTime - delay);
        if(delay > 0){
            AudioManager.instance.Play("Holy_Aleluya");
            yield return new WaitForSeconds(delay);
        }
        Detonate();
    } private IEnumerator Timer2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
