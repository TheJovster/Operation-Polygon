using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    [SerializeField] private Transform objectToFollow;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float lookSpeed = 10f;

    private void LookAtTarget() 
    {
        Vector3 lookDirection = objectToFollow.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookSpeed * Time.deltaTime); ;
    }

    private void MoveToTarget() 
    {
        Vector3 targetPosition = objectToFollow.position +
                                 objectToFollow.forward * offset.z
                                 + objectToFollow.right * offset.x
                                 +objectToFollow.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }

}
