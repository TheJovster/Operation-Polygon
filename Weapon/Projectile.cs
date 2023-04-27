using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using OperationPolygon.Core;


namespace OperationPolygon.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private GameObject hitGenericParticleFX;
        [SerializeField] private GameObject[] hitDecals;
        [SerializeField] private float decalLifetime = 3f;


        private Rigidbody rigidBody;
        private Weapon playerWeapon;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            playerWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon").GetComponent<Weapon>();
        }

        private void Start()
        {
            rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);
            Destroy(gameObject, 6f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == "EnemyDrone") 
            {
                other.
                collider.
                GetComponent<Health>()?.
                TakeDamage(
                GameObject.FindGameObjectWithTag("PlayerWeapon").
                GetComponent<Weapon>().
                GetWeaponDamage());
                //not very performant
                //difficult to read, but I think it's useable in such a small project
                //I'd go for another solution for a more hardware intensive project.
                Destroy(this.gameObject);
            }

            else if (other.gameObject.tag == "EnemySoldier")
            {
                other.
                collider.
                GetComponent<Health>()?.
                TakeDamage(playerWeapon.
                GetWeaponDamage());
                //not very performant
                //difficult to read, but I think it's useable in such a small project
                //I'd go for another solution for a more hardware intensive project.
                Destroy(this.gameObject);
            } 
            else if(other.gameObject.tag == "EnemyZombie") 
            {
                other.
                collider.
                GetComponent<Health>()?.
                TakeDamage(
                playerWeapon.
                GetWeaponDamage());
                //not very performant
                //difficult to read, but I think it's useable in such a small project
                //I'd go for another solution for a more hardware intensive project.
                Destroy(this.gameObject);
            }
            else 
            {
                var hitFXInstance = Instantiate(hitGenericParticleFX, transform.position, Quaternion.identity);
                foreach(var hitDecal in hitDecals) 
                {
                    var decalInstance = Instantiate(hitDecal, transform.position, hitDecal.transform.rotation);
                    Destroy(decalInstance, decalLifetime);
                }
                Destroy(hitFXInstance, hitFXInstance.GetComponent<ParticleSystem>().main.duration);
                Destroy(this.gameObject);
            }
        }
    }
}
