using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
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
    public float autoDestroyOnHeight = -50f;
    private float rotationAmount;
    private bool soundAlternator;
    private Rigidbody2D rb;
    private CircleCollider2D cirColider;
    private DamageHandler target;

    void Start()
    {
        AudioManager.instance.Play(spawnSoundName);
        
        //Add initial force once to make a parabolic trajectory
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * launchForce * rb.mass);

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

        //Get all colliders in a circular area of radius "explosionRadius"
        Collider2D[] collisions = Physics2D.OverlapCircleAll( transform.position, explosionRadius);

        //Loop thru each collider
        foreach(Collider2D col in collisions)
        {
            //get rigidbody and add a repulsive force
            rb = col.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Rigidbody2DExtension.AddExplosionForce(rb, transform.position, explosionRadius, explosionForce, extraUpForce);
            }

            //get damageHandler script and apply damage depending on distance
            target = col.GetComponent<DamageHandler>();
            if(target != null)
            {
                var dir = (col.transform.position - transform.position);
                float wearoff = 1 - (dir.magnitude / explosionRadius);
                target.TakeDamage( (int)(damageToPlayer * wearoff), damageToProps);
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
    }
}
