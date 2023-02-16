using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);
        Destroy(gameObject, 6f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
