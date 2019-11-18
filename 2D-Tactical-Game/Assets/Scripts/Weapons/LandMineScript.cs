using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineScript : MonoBehaviour
{
    public float timer = 2.4f;
    public bool isLoyal = false;
    public int damageToPlayer = 50;
    public int damageToProps = 50;
    public float launchForce = 1000f;
    public float explosionForce = 1000f;
    public float explosionRadius = 6f;
    public GameObject impactEffect;
    public float autoDestroyOnHeight = -50f;
    public string spawnSoundName = "Grenade_Launcher";
    public string explosionSoundName = "Mine_Blast";
    public string impactSound1 = "Metal_Hit_High";
    public string impactSound2 = "Metal_Hit_Low";

    private bool isMineActive = false;
    private bool soundAlternator;
    private Rigidbody2D rb;
    private Animator anim;
    private DamageHandler target;
    private PlayerSettings ps;
    private int teamOwner;


    private void Awake() {
        if(isLoyal){
            teamOwner = GlobalVariables.Instance.GM.currTeamTurn;
        }
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();

        AudioManager.instance.Play(spawnSoundName);
        //Add initial force once to make a parabolic trajectory
        rb.AddForce(transform.right * launchForce * rb.mass);
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

    private void OnTriggerEnter2D(Collider2D other) {
        
        //ACtivates once and only with player presence
        if(isMineActive == false && other.transform.tag == "Player"){
            //Check if this mine recognizes friendly players
            if(isLoyal){
                //Get PlayerSettings and check if player team in question is the owner of the mine
                ps = other.gameObject.GetComponent<PlayerSettings>();
                if(ps != null && ps.teamID == teamOwner){
                    //teamOwner can't activate mine.
                    return;
                }
            }
            isMineActive = true;
            anim.SetTrigger("Active");
            AudioManager.instance.Play("Proximity_Alarm");
            StartCoroutine(Timer(timer));
        }
    }

    void Detonate()
    {
        //Explosion Sound
        AudioManager.instance.Stop("Proximity_Alarm");
        AudioManager.instance.Play(explosionSoundName);

        //Create an impact effect like an explosion
        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        //Get all colliders in a circular area of radius "explosionRadius"
        Collider2D[] collisions = Physics2D.OverlapCircleAll( transform.position, explosionRadius);

        //Loop thru each collider
        foreach(Collider2D col in collisions)
        {
            //get rigidbody and add a repulsive force
            rb = col.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Rigidbody2DExtension.AddExplosionForce(rb, transform.position, explosionRadius, explosionForce);
            }

            //get damageHandler script and apply damage depending on distance
            target = col.GetComponent<DamageHandler>();
            if(target != null)
            {
                var dir = (col.transform.position - transform.position);
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                target.TakeDamage( Mathf.Max(0, (int)(damageToPlayer * wearoff)), damageToProps);
            }
        }

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D other) {
        if(soundAlternator){
            AudioManager.instance.Play(impactSound1);
        }else{
            AudioManager.instance.Play(impactSound2);
        }
        soundAlternator = !soundAlternator;
    }

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Detonate();
    }
}
