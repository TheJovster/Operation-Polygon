using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRagdollCleaner : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;

    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, lifeTime);
    }
}
