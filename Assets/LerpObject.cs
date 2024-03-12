using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpObject : MonoBehaviour
{
    float lerpTime = 1f;
    float currentLerpTime;

    float moveDistance = 10f;

    Vector3 startPos;
    public GameObject endPos;

    Coroutine movementCoroutine;

    public AnimationCurve animationCurve;
    protected void Start()
    {
        startPos = transform.position;        
        StartCoroutine(MovementCoroutine());
        //MovementCoroutine();
    }

    protected void Update()
    {
        //reset when we press spacebar
        /*if (Input.GetKeyDown(KeyCode.Z))
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }
            movementCoroutine = 
        }*/


    }

    IEnumerator MovementCoroutine()
    {
        while (true)
        {

            currentLerpTime = 0f;

            while (currentLerpTime < lerpTime)
            {
                currentLerpTime += Time.deltaTime;
                float t = currentLerpTime / lerpTime;
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.position = Vector3.Lerp(startPos, endPos.transform.position, t);
                //transform.position = Vector3.Lerp(startPos, endPos.transform.position, animationCurve.Evaluate(currentLerpTime));
                yield return null;
            }

            yield return new WaitForSeconds(1);

            currentLerpTime = 0f;

            while (currentLerpTime < lerpTime)
            {
                currentLerpTime += Time.deltaTime;
                float t = currentLerpTime / lerpTime;
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                transform.position = Vector3.Lerp(endPos.transform.position, startPos, t);
                //transform.position = Vector3.Lerp(endPos.transform.position, startPos, animationCurve.Evaluate(currentLerpTime));
                yield return null;
            }

            yield return new WaitForSeconds(1);
        }

    }
}