using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePunch : MonoBehaviour
{
    public int damageToPlayer = 40;
    public int damageToProps = 100;
    public float delayTime = 1.5f;
    public float horizontalForce = 2000f;
    public float verticalForce = 250f;
    public string swingSound = "Hammer_Spin";
    public string hitSound = "Hammer_Hit";
    private Collider2D thisCollider;
    private DamageHandler target;
    private Rigidbody2D rb;
    
    void Start()
    {
        thisCollider = gameObject.GetComponent<Collider2D>();
        thisCollider.enabled = false; //make sure the collider is deactivated
        Invoke("EnableCollider", delayTime); //enable collider after animation if there is any
        AudioManager.instance.Play(swingSound);
        Destroy(gameObject, delayTime + 0.03f);
    }

    private void EnableCollider(){
        thisCollider.enabled = true; //enable collider
        AudioManager.instance.Stop(swingSound);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        AudioManager.instance.Play(hitSound);

        //Get DamageHandler script if ay from colliding object
        target = other.gameObject.GetComponent<DamageHandler>();
        //Deal damage
        if(target != null){
            target.TakeDamage(damageToPlayer, damageToProps);
        }

        //get rigidbody to add a force to reflect the impact
        rb = other.transform.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            //Only add force if component have a rigidbody2D
            rb.AddForce(transform.up * verticalForce);
            rb.AddForce(transform.right * horizontalForce);
        }
        WatchEnemy(other.gameObject);
        thisCollider.enabled = false; //Deactivate Collider
    }

    private void WatchEnemy(GameObject enemy){
        CameraController.instance.soldier = enemy;
    }
}
