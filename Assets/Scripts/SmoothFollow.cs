using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
    
    public float smoothTime = 5f;
    public float maxSpeed = 5f;
    public float rotateSpeed = 5f;
    public float forwardOffset = 3f;
    public float upOffset = 2f;
    public Transform target;
    Vector3 refVelocity;
    Vector3 previousTargetPosition;

	void FixedUpdate () {
        if (target)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position + target.forward*forwardOffset + target.up*upOffset, ref refVelocity, smoothTime, maxSpeed, Time.deltaTime*maxSpeed);
            //transform.rotation = nearestLocation.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    public void JumpToTarget()
    {
        transform.position = target.position + target.forward * forwardOffset + target.up * upOffset;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = targetRotation;
    }
}
