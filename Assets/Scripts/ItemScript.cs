using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{

    protected Rigidbody2D rb;
    float velocityAirFrictionSlowdown = 0.994f;
    float velocityFrictionSlowdown = 0.97f;
    private void FixedUpdate()
    {
        
            rb.velocity *= new Vector2(velocityFrictionSlowdown, velocityFrictionSlowdown);
             
            rb.angularVelocity *= velocityFrictionSlowdown;
       
      

    }
    //------------------Functions to override-----------------//
    //------------Self actions-----------//
    public virtual void Fire()
    {
    }
    //public virtual void RandomSpawn()




    //------Interactions with Player-----//
    public virtual void AttachToPlayer()
    {

    }
    public virtual void DetachFromPlayer()
    {

    }
    public virtual void PlaceInThePlayerBack()
    {

    }
    public virtual void PlaceInThePlayerHands()
    {

    }
}
