using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] PlayerController playerController;
    [SerializeField] int offset = 15;
    [SerializeField] Vector2 sizeRange = new Vector2(7, 15);[SerializeField] float lerpingSpeed = .9f;
    [SerializeField] float smoothTime = .1f;

    private Vector3 velocity = Vector3.zero;

    Camera camera = null;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        if (!target)
            return;
        Vector3 targetPos = target.position + new Vector3(0f,0f,-offset);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        if(playerController.IsShooting())
         {
            StartCoroutine(LerpCamera(sizeRange.y, lerpingSpeed));
         }
         else
            StartCoroutine(LerpCamera(sizeRange.x, lerpingSpeed));


    }
    
    IEnumerator LerpCamera(float targetSize, float durationUnit)
    {
        float time = 0;
        float startValue = camera.orthographicSize;
        while(time < durationUnit)
        {
            camera.orthographicSize = Mathf.Lerp(startValue, targetSize, time / durationUnit);
            time += Time.deltaTime;
            yield return null;  
        }

        camera.orthographicSize = targetSize;
    }
}
