using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySoldierController : MonoBehaviour
{
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        LookAtPlayer();
        //TODO: I need to write a custom IK script for the NPCs.
        //the IK script needs to be separate

    }

    private void LookAtPlayer() 
    {
        Vector3 playerPositionAim = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(playerPositionAim);
    }

    //SearchBehaviour

    //ChaseBehaviour

    //EngageBehaviour

    //RetreatBehaviour

    //PatrolBehaviour

    //???
}
