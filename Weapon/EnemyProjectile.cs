using OperationPolygon.Core;
using UnityEngine;

namespace OperationPolygon.AI 
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private GameObject hitGenericParticleFX;

        private Rigidbody rigidBody;
        private EnemyWeapon weapon;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            weapon = GetComponentInParent<EnemyWeapon>();
        }

        private void Start()
        {
            rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);
            Destroy(gameObject, 6f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Health>().TakeDamage(3);
                Destroy(this.gameObject);
            }
            else 
            {
                GameObject genericHitFX = Instantiate(hitGenericParticleFX);
                Destroy(genericHitFX, hitGenericParticleFX.GetComponent<ParticleSystem>().main.duration);
            }

        }
    }
}
