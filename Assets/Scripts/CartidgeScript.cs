using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartidgeScript : MonoBehaviour
{
    [SerializeField] float lifeTime = 4f;
    float velocityFrictionSlowdown = 0.92f;//Decrease this to increase slowness   0.993f for Update
    float cartidgeScaleSlowDownPercent = 0.01f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
    }
    // Update is called once per frame
    void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;

        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    private void FixedUpdate()
    {
        transform.localScale -= new Vector3(transform.localScale.x* cartidgeScaleSlowDownPercent, transform.localScale.y*cartidgeScaleSlowDownPercent);
        rb.velocity *= new Vector2(velocityFrictionSlowdown, velocityFrictionSlowdown);
        rb.angularVelocity *= velocityFrictionSlowdown;
    }
}
