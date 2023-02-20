using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //components
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Health health;
    

    void Awake()
    {
        
    }

    void Update()
    {
        LookAtTarget();
    }

    private Vector3 GetLookAtPosition() 
    {
        Transform target;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 lookAtTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
        return lookAtTarget;
    }

    private void LookAtTarget() 
    {
        transform.LookAt(GetLookAtPosition(), Vector3.up);
    }
}
