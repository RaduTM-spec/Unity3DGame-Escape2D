using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowingScript : MonoBehaviour
{
    ZombieScript zmScript = null;
    Rigidbody2D rb = null;
    private void Awake()
    {
        zmScript = GetComponent<ZombieScript>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        try
        {
            if (zmScript.searchCommand && zmScript.stunned <= 0f)
            {
                Search(zmScript.GetNearestTargetVar(), zmScript.GetMovementSpeed());
            }
        }
        catch { }
            
    }

    private void Search(Transform target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        Quaternion toDirection = Quaternion.LookRotation(Vector3.forward, target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toDirection, 120 * Time.deltaTime);
    }

}
