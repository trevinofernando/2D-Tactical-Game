using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderGunScript : MonoBehaviour
{
    public int damageToPlayer = 25;
    public int damageToProps = 1;
    public float pushForwardForce = 50f;
    public float pushBackForce = 50f;
    public float upForce = 300f;

    private void Start() {
        AudioManager.instance.Play("ThunderGun");

    //Push player back

        //Direct cast from Vector3s to Vector2s
        Vector2 origin = transform.position; //from source
        Vector2 direction = -transform.right; //opposite to facing direction 

        //Send a ray and store the information in hit
        RaycastHit2D hit = Physics2D.Raycast(origin, direction);

        if(hit.transform != null){
            Debug.Log(hit.transform.tag);
            //get rigidbody to add a force to reflect the impact
            Rigidbody2D rb = hit.transform.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                //Only add force if component have a rigidbody2D
                rb.AddForce(direction * pushBackForce, ForceMode2D.Impulse);
            }
            
        }

        Destroy(gameObject, 0.4f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        DamageHandler target = other.transform.GetComponent<DamageHandler>();
        if(target != null){
            target.TakeDamage( damageToPlayer , damageToProps);
        }

        Rigidbody2D rb = other.transform.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                //Only add force if component have a rigidbody2D
                rb.AddForce(transform.up * upForce);
                rb.AddForce(transform.right * pushForwardForce, ForceMode2D.Impulse);
            }

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

}
