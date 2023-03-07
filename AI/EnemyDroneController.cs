using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationPolygon.AI
{
    public class EnemyDroneController : MonoBehaviour
    {
        private EnemyDetector detector;
        private NavMeshAgent navMeshAgent;
        [SerializeField] private EnemyWeapon droneGun;
        [SerializeField] private Vector3 aimOffset;
        private Transform playerTransform;

        [SerializeField] private float attackDistance = 7f;

        private void Awake()
        {
            detector = GetComponentInChildren<EnemyDetector>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (detector.GetAggro()) 
            {
                MoveToPlayer();
                LookAtPlayer();
                AimAtPlayer();
            }
            else 
            {
                PatrolBehavior();
            }

        }

        private void LookAtPlayer() 
        {
            transform.LookAt(playerTransform.position + aimOffset);
        }

        private void AimAtPlayer() 
        {
            Vector3 aimAtPosition = new Vector3(playerTransform.position.x, playerTransform.transform.position.y, playerTransform.position.z);
            droneGun.transform.LookAt(aimAtPosition + aimOffset);
            //if physics raycast has been hitting the player for x amount of time, shoot;

        }

        private void MoveToPlayer() 
        {
            navMeshAgent.SetDestination(playerTransform.position);
            if(DistanceToPlayer() <= attackDistance) 
            {
                navMeshAgent.isStopped = true;
                TryToAttack();
                Debug.Log("Attacking the player");
                //attack to behaviour
            }
        }

        private void TryToAttack() 
        {
            droneGun.Shoot();
        }

        private void PatrolBehavior() 
        {

        }

        private float DistanceToPlayer() 
        {
            return Vector3.Distance(transform.position, playerTransform.position);
        }

    }
}

