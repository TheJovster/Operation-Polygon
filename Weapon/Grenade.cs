using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class Grenade : MonoBehaviour
    {
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float explosionForce = 100f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject explosionSFX;
        private Weapon playerWeapon;

        private void Start()
        {
            playerWeapon = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Weapon>();
        }

        private void Explode(int damageToDeal)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
            foreach (Collider collider in colliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damageToDeal);
                }
            }

        }

        private IEnumerator ExplodeRigidBodies()
        {
            yield return new WaitForSeconds(.05f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                }
            }
            yield return new WaitForSeconds(.05f);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Ground" || other.gameObject.tag == "EnemyZombie") 
            {
                GameObject sfxInstance = Instantiate(explosionSFX, transform.position, Quaternion.identity);
                Explode(playerWeapon.GetWeaponDamage());
                StartCoroutine(ExplodeRigidBodies());
                Destroy(sfxInstance, 2f);
            }
        }
    }
}
