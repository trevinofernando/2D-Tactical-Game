using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePunch : MonoBehaviour
{
    public int damageToPlayer = 40;
    public int damageToProps = 100;
    public float delayTime = 1.5f;
    public float horizontalForce = 2000f;
    public float verticalForce = 2000f;
    private Collider2D thisCollider;
    private DamageHandler target;
    private Rigidbody2D rb;
    void Start()
    {
        thisCollider = gameObject.GetComponent<Collider2D>();
        thisCollider.enabled = false; //make sure the collider is deactivated
        Invoke("EnableCollider", delayTime); //enable collider after animation if there is any
        Destroy(gameObject, delayTime + 0.03f);
    }

    private void EnableCollider(){
        thisCollider.enabled = true; //enable collider
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
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
        thisCollider.enabled = false; //Deactivate Collider
    }
/* 
    private void OnTriggerEnter2D(Collider2D other) {
        //I decided to use a coroutine because I can pass arguments, and I want to pass arguments so I can call this function from other scripts
        StartCoroutine(DamageArea(other.transform, damageToPlayer, damageToProps, rayLifeTime));
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
    }
*/
    public IEnumerator DamageArea(Transform obj, int _damageToPlayer, int _damageToProps, float delayTime){
        //Wait a few seconds and start
        yield return new WaitForSeconds(delayTime);
        
        target = obj.GetComponent<DamageHandler>();
        if(target != null){
            target.TakeDamage( _damageToPlayer , _damageToProps);
        }
        Destroy(gameObject);
    }
}
