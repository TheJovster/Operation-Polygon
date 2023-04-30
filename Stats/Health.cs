using OperationPolygon.AI.Control;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
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
        [SerializeField] private AudioSource headAudioSource;
        private Animator animator;
        private FragCount fragCount;
        [Header("FX")]
        [SerializeField] private GameObject onDestroyParticle;
        [SerializeField] private GameObject onHitParticle;
        [SerializeField] private GameObject ragdollPrefab;
        [SerializeField] private AudioClip onDestroySFX;
        [SerializeField] private AudioClip[] onHitSFX;
        [SerializeField] private AudioClip[] gruntSFX; //add this to enemies later.
        //sound
        [Header("Additive Variables")]
        [SerializeField] private Vector3 VFXOffset;

        
        private int takeDamageAnimHash = Animator.StringToHash("GetHit");

        [Header("NPC Variables")]
        [SerializeField] private float staminaToAdd = 5f;


        [SerializeField] private float fadeOutTime = 10f;
        private float elapsedTime = 0f;

        //precache - might be unperformant/memory drain
        [SerializeField]SkinnedMeshRenderer[] skinnedMeshRenderers;
        [SerializeField]MeshRenderer[] meshRenderers;

        private void Awake()
        {
            currentHealth = maxHealth;
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();

            skinnedMeshRenderers = PreCacheSkinnedMeshRenderers();
            meshRenderers = PreCacheMeshRenderers();
            fragCount = GameObject.FindGameObjectWithTag("Player")?.GetComponent<FragCount>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {

        }

        public void TakeDamage(int damageToTake) 
        {
            if (isAlive) 
            {
                currentHealth -= damageToTake;

                if(this.gameObject.tag == "EnemyDrone") 
                {
                    int gruntIndex = Random.Range(0, gruntSFX.Length);
                    headAudioSource.PlayOneShot(gruntSFX[gruntIndex]);
                }

                if(this.gameObject.tag == "EnemyZombie") 
                {
                    animator.SetTrigger("GetHit");
                    this.gameObject.GetComponent<EnemyZombieController>().SetAggro();
                    if(this.gameObject.GetComponentInParent<HordeBehaviourScript>() != null) 
                    {
                        this.gameObject.GetComponentInParent<HordeBehaviourScript>().HordeAlert();
                    }
                }
                else if(this.gameObject.tag == "EnemySoldier") 
                {
                    animator.SetTrigger("GetHit");
                }
                else if(this.gameObject.tag == "Player") 
                {
                    onHitParticle.GetComponent<ParticleSystem>().Play();
                    int onHitSFXIndex = Random.Range(0, onHitSFX.Length);
                    audioSource.PlayOneShot(onHitSFX[onHitSFXIndex]);
                    int gruntSFXIndex = Random.Range(0, gruntSFX.Length);
                    headAudioSource.PlayOneShot(gruntSFX[gruntSFXIndex]);
                    StartCoroutine(PlayerTriggerGetHit());
                }
            }
            if(currentHealth <= 0) 
            {
                isAlive = false;
                Die();
            }
        }

        private void Die() 
        {
            currentHealth = 0;
            audioSource.PlayOneShot(onDestroySFX);
            var fxInstance = Instantiate(onDestroyParticle, transform.position + VFXOffset, Quaternion.identity); 
            Destroy(fxInstance, 1f);
            transform.GetComponent<Collider>().enabled = false; //had to change it to a general query.
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
                    DisableMeshRenderers();
                    NavMeshAgent navMesh = transform.GetComponent<NavMeshAgent>();
                    navMesh.enabled = false;
                    GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
                    fragCount.IncrementFragCount();
                    foreach (var rigidBody in ragdoll.GetComponentsInChildren<Rigidbody>())
                    {
                        rigidBody.AddExplosionForce(100f, ragdoll.transform.position, 10f);
                        StartCoroutine(FadeOutMesh(ragdoll));
                    }

                }
            }
            else if (!isHumanoid)
            {
                DisableMeshRenderers();
                Destroy(gameObject, onDestroySFX.length + .05f);
            }

            //TODO: Add Ragdoll functionality
            //TODO Done: Ragdolls added.
        }

        private void DisableMeshRenderers()
        {
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                skinnedMeshRenderer.enabled = false;
            }
            if (meshRenderers.Length > 0)
            {
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.enabled = false;
                }
            }
        }

        private IEnumerator FadeOutMesh(GameObject ragdoll) 
        {
            SkinnedMeshRenderer skinnedMeshRenderer = ragdoll.GetComponentInChildren<SkinnedMeshRenderer>();
            Material[] materials = skinnedMeshRenderer.materials;

            // Loop through all materials and set their rendering mode to transparent
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                materials[i].SetInt("_ZWrite", 0);
                materials[i].DisableKeyword("_ALPHATEST_ON");
                materials[i].EnableKeyword("_ALPHABLEND_ON");
                materials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                materials[i].renderQueue = 3000;
            }

            // Gradually decrease the alpha value of the color property over time
            while (elapsedTime < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
                foreach (Material material in materials)
                {
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set the alpha value to 0 and disable the skinned mesh renderer
            foreach (Material material in materials)
            {
                Color color = material.color;
                color.a = 0f;
                material.color = color;
            }
            skinnedMeshRenderer.enabled = false;

            yield return new WaitUntil(() => skinnedMeshRenderer.enabled == false);
            Destroy(ragdoll.gameObject);
            yield return new WaitForSeconds(.5f);
            Destroy(this.gameObject);
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

        private SkinnedMeshRenderer[] PreCacheSkinnedMeshRenderers() 
        {
            SkinnedMeshRenderer[] renderers;
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            return renderers;
        }

        private MeshRenderer[] PreCacheMeshRenderers() 
        {
            MeshRenderer[] renderers;

            renderers = GetComponentsInChildren<MeshRenderer>();
            return renderers;
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

        public float GetStaminaToAdd() 
        {
            return staminaToAdd;
        }

        public int GetCurrentHealth() 
        {
            return currentHealth;
        }

        public int GetMaxHealth() 
        {
            return maxHealth;
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
