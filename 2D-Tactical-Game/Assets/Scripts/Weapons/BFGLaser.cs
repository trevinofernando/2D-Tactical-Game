using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFGLaser : MonoBehaviour
{
    public int damageToPlayer = 20;
    public int damageToProps = 100;
    public float rayLifeTime = 1f;

    private void Start() {
        Destroy(gameObject, rayLifeTime + 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //I decided to use a coroutine because I can pass arguments, and I want to pass arguments so I can call this function from other scripts
        StartCoroutine(DamageArea(other.transform, damageToPlayer, damageToProps, rayLifeTime));
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public IEnumerator DamageArea(Transform obj, int _damageToPlayer, int _damageToProps, float delayTime){

        //Wait a few seconds and start
        yield return new WaitForSeconds(delayTime);

        DamageHandler target = obj.GetComponent<DamageHandler>();
        if(target != null){
            target.TakeDamage( _damageToPlayer , _damageToProps);
        }
        Destroy(gameObject);
    }


}
