using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationPolygon.AI.Control 
{
    public class EnemyController : MonoBehaviour
    {
        //components
        [SerializeField] private NavMeshAgent navMesh;
        [SerializeField] private Health health;
        private EnemyDetector detector;
        private Animator animator;

        //target
        Transform target;

        //private variables
        private string animSpeedID = "Speed";
        private int animSpeedHash;
        //serialized variables

        //public variables

        void Awake()
        {
            navMesh = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            detector = GetComponentInChildren<EnemyDetector>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            animSpeedHash = Animator.StringToHash(animSpeedID);
        }

        void Update()
        {
            if (detector.GetAggro()) 
            {
                LookAtTarget();
                MoveToTarget();
                //move to target
            }
            
        }

        private Vector3 GetLookAtPosition()
        {
            Vector3 lookAtTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
            return lookAtTarget;
        }

        private void LookAtTarget()
        {
            transform.LookAt(GetLookAtPosition(), Vector3.up);
        }

        private void MoveToTarget() 
        {
            navMesh.SetDestination(target.position);
        }

        //animation handling and events
        private void HandleAnimation() 
        {
            animator.SetFloat(animSpeedHash, navMesh.velocity.magnitude);


        }
    }
}
