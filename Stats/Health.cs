using OperationPolygon.AI.Control;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//I decided to put the Health component into the core namespace
//because I have a hunch that a few more scripts are going to rely on it.
//as such, the core is going to be using all of the namespaces in the assembly
namespace OperationPolygon.Core 
{
    public class Health : MonoBehaviour 
    {
        [SerializeField] private int currentHealth = 100; //ints are cheaper than floats
        [SerializeField] private int maxHealth;
        [SerializeField] private bool isAlive = true; //serialize field for testing purposes - is alive is going to be used A LOT later on
        [SerializeField] private bool isHumanoid = true;

        //components
        [Header("Components")]
        [SerializeField] private AudioSource audioSource;
        private Animator animator;
        [Header("FX")]
        [SerializeField] private GameObject onDestroyParticle;
        [SerializeField] private GameObject onHitParticle;
        [SerializeField] private GameObject ragdollPrefab;
        [SerializeField] private AudioClip onDestroySFX;
        [SerializeField] private AudioClip[] onHitSFX;
        //sound
        [Header("Additive Variables")]
        [SerializeField] private Vector3 VFXOffset;

        
        private int takeDamageAnimHash = Animator.StringToHash("GetHit");

        [Header("NPC Variables")]
        [SerializeField] private float staminaToAdd = 5f;

        private void Awake()
        {
            currentHealth = maxHealth;
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {

        }

        public void TakeDamage(int damageToTake) 
        {
            if (isAlive) 
            {
                currentHealth -= damageToTake;
                animator.SetTrigger("GetHit");
                if(this.gameObject.tag == "Enemy") 
                {
                    GameObject.FindGameObjectWithTag("Player").
                        GetComponentInChildren<Stamina>().
                        RegenStamina(staminaToAdd);
                    this.gameObject.GetComponent<EnemyController>().SetAggro();
                    if(this.gameObject.GetComponentInParent<HordeBehaviourScript>() != null) 
                    {
                        this.gameObject.GetComponentInParent<HordeBehaviourScript>().HordeAlert();
                    }
                }
                else if(this.gameObject.tag == "Player") 
                {
                    onHitParticle.GetComponent<ParticleSystem>().Play();
                    StartCoroutine(PlayerTriggerGetHit());
                }
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
            transform.GetComponent<Collider>().enabled = false; //had to change it to a genral query.
            if (isHumanoid) 
            {
                if(gameObject.tag == "Player") 
                {
                    animator.SetBool("Die", true);
                    FindObjectOfType<GameManager>().SetGameOver();
                    //game over functionality
                }
                if(gameObject.tag != "Player") 
                {
                    SkinnedMeshRenderer meshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
                    NavMeshAgent navMesh = transform.GetComponent<NavMeshAgent>();
                    navMesh.enabled = false;
                    meshRenderer.enabled = false;
                    var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
                    foreach (var rigidBody in ragdoll.GetComponentsInChildren<Rigidbody>())
                    {
                        rigidBody.AddExplosionForce(100f, ragdoll.transform.position, 10f);
                        Destroy(gameObject, onDestroySFX.length + .05f);
                    }
                }

            }
            else if (!isHumanoid) 
            {
                transform.GetComponent<MeshRenderer>().enabled = false;
                Destroy(gameObject, onDestroySFX.length + .05f);
            }
            
            //TODO: Add Ragdoll functionality
            //TODO Done: Ragdolls added.
        }

        private void OnCollisionEnter(Collision other) 
            //when hit, instantiates the hit effects and then destroys the instance after the assigned duration
        {
            if(other.gameObject.tag == "Projectile") 
            {
                var instance = Instantiate(onHitParticle, other.transform.position, Quaternion.identity);
                Destroy(instance, instance.GetComponent<ParticleSystem>().main.duration);
                int onHitSFXIndex = Random.Range(0, onHitSFX.Length);
                audioSource.PlayOneShot(onHitSFX[onHitSFXIndex]);
            }
        }

        public void AddHealth(int healthToAdd) 
        {
            currentHealth += healthToAdd;
            if(currentHealth >= maxHealth) 
            {
                currentHealth = maxHealth;
                
            }
        }

        public float GetHealthPercentage() 
        {
            float healthPercentage = (float)currentHealth / (float)maxHealth;
            return healthPercentage;
        }

        //public getters
        public bool IsAlive() 
        {
            return isAlive;
        }

        //player-specific coroutines
        private IEnumerator PlayerTriggerGetHit() 
        {
            animator.SetLayerWeight(2, 1);
            animator.Play(takeDamageAnimHash, 2);
            yield return new WaitForSeconds(0.766f);
            animator.SetLayerWeight(2, 0);
        }
    }

}
