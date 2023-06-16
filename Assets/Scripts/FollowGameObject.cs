using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FollowGameObject : MonoBehaviour
{
    //=========
    //VARIABLE
    //=========
    public Transform objectToFollow;
    public Vector3 offset;
    public bool lerp = false;
    public float lerpSpeed = 10;
    public AnimationCurve lerpCurve;
    private float lerpCurveTimePosition;
    private Vector3 lerpStartPoint;
    //=========
    //FONCTION
    //=========
    private void FixedUpdate()
    {
        if (transform.position != objectToFollow.position + offset)
        {
            if (!lerp)
                transform.position = objectToFollow.position + offset;
            else
            {
                lerpCurveTimePosition += Time.deltaTime;
                transform.position = Vector3.Lerp(lerpStartPoint, objectToFollow.position + offset, lerpCurve.Evaluate(lerpCurveTimePosition));
                //transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position + offset, lerpSpeed);
            }
        }
        else
        {
            lerpStartPoint = transform.position;
            lerpCurveTimePosition = 0;
        }
    }
}
