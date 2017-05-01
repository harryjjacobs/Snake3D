using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
    
    public float smoothTime = 5f;
    public float maxSpeed = 5f;
    public float rotateSpeed = 5f;
    public float forwardOffset = 3f;
    public float upOffset = 2f;
    public float smallAngleThreshold = 10f;
    public float smallAngleMovementAmount = 0.5f;
    public Transform target;
    Vector3 refVelocity;
    Vector3 refVelocity2;

    void LateUpdate () {
        if (target)
        {
            Vector3 targetPosition = target.position + target.forward * forwardOffset + target.up * upOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothTime, maxSpeed, Time.deltaTime*maxSpeed);

            Quaternion targetRotation = Quaternion.LookRotation((target.position + target.forward * GameManager.GetSnakeSpeed() * Time.deltaTime) - transform.position, target.up);
            float angle = Quaternion.Angle(transform.rotation, targetRotation); //Rotate faster if we have to move a further distance
            if (angle < smallAngleThreshold)
            {
                angle = smallAngleMovementAmount;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed * angle);
        }
    }

    public void JumpToTarget()
    {
        transform.position = target.position + target.forward * forwardOffset + target.up * upOffset;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = targetRotation;
    }
}
