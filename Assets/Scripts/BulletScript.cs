using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float lifeTime = 4f;
    float damage = 1f;

    //Impact Particle Systems
    ParticleSystem blood = null;
    ParticleSystem wallScratch = null;
    // Update is called once per frame
    private void Awake()
    {
        blood = transform.Find("BloodPS").GetComponent<ParticleSystem>();
        wallScratch = transform.Find("WallScratchPS").GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if(lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;

        

        if(collision.collider.CompareTag("Zombie"))
        {
            transform.parent = collision.collider.transform;
            blood.Play();
            ZombieScript zm = collision.collider.GetComponent<ZombieScript>();

            zm.DealDamage(damage);
            zm.SetMovementSpeed(0.5f);

            Destroy(gameObject, blood.GetComponent<ParticleSystem>().main.duration);
        }
        else if(collision.collider.CompareTag("Wall"))
        {
            wallScratch.Play();
            Destroy(gameObject, wallScratch.GetComponent<ParticleSystem>().main.duration);
        }
        else
        {
            Destroy(gameObject);
        }


        GetComponent<CapsuleCollider2D>().enabled = false;
        
    }

    public void SetDamage(float damage)
    {
       this.damage = damage;
    }
}
