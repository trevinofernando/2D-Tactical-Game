using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int health = 100;
    public GameObject deathEffect;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        anim.SetTrigger("takeDamage");

        if(health <= 0)
        {
            anim.SetTrigger("die");
            Die();
        }
    }

    void Die()
    {
        if(deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
