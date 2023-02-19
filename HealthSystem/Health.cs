using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I decided to put the Health component into the core namespace
//because I have a hunch that a few more scripts are going to rely on it.
//as such, the core is going to be using all of the namespaces in the assembly
namespace OperationPolygon.Core 
{
    public class Health : MonoBehaviour 
    {
        [SerializeField] private int currentHealth; //ints are cheaper than floats
        [SerializeField] private int maxHealth;
        [SerializeField] private bool isAlive = true; //serialize field for testing purposes - is alive is going to be used A LOT later on
        [SerializeField] private bool isHumanoid = true;

        //components
        [Header("Components")]
        [SerializeField] private AudioSource audioSource;
        [Header("FX")]
        [SerializeField] private GameObject onDestroyParticle;
        [SerializeField] private GameObject ragdollPrefab;
        [SerializeField] private AudioClip onDestroySFX;
        //sound
        [Header("Additive Variables")]
        [SerializeField] private Vector3 VFXOffset;

        private void Awake()
        {
            currentHealth = maxHealth;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {

        }

        public void TakeDamage(int damageToTake) 
        {
            if (isAlive) 
            {
                currentHealth -= damageToTake;
            }
            //particle and sounds effects
            //animation triggers
            if(currentHealth <= 0) 
            {
                isAlive = false;
                Die();
            }
        }

        //TOOD: public void  RestoreHealth(int restoreAmount) {}

        private void Die() 
        {

            currentHealth = 0;
            audioSource.PlayOneShot(onDestroySFX);
            var fxInstace = Instantiate(onDestroyParticle, transform.position + VFXOffset, Quaternion.identity); 
            Destroy(fxInstace, 1f);
            Debug.Log(gameObject.name + " has taken too much damage and needs a break.");
            transform.GetComponent<CapsuleCollider>().enabled = false;
            if (isHumanoid) 
            {
                SkinnedMeshRenderer meshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
                meshRenderer.enabled = false;
                var ragdoll = Instantiate(ragdollPrefab, transform.position, Quaternion.identity);
                foreach (var rigidBody in ragdoll.GetComponentsInChildren<Rigidbody>())
                {
                    rigidBody.AddExplosionForce(100f, ragdoll.transform.position, 10f);
                }
            }
            else if (!isHumanoid) 
            {
                transform.GetComponent<MeshRenderer>().enabled = false;
            }
            Destroy(gameObject, onDestroySFX.length + .05f);
            //TODO: Add Ragdoll functionality
        }

    }

}
