using OperationPolygon.Combat;
using UnityEngine;


namespace OperationPolygon.Core 
{
    public class Pickup : MonoBehaviour
    {
        [Header("Resource Type")]
        [SerializeField] private bool isAmmo;
        [SerializeField] private bool isHealth;

        [Header("Resource Ammount")]
        [SerializeField] private int ammoToAdd;
        [SerializeField] private int healthToAdd;

        [Header("VFX")]
        [SerializeField] private GameObject ammoVFX;
        [SerializeField] private GameObject healthVFX;

        [Header("SFX")]
        [SerializeField] private AudioClip ammoSFX;
        [SerializeField] private AudioClip healthSFX;

        //prefab visual rotation
        private float rotationSpeed = 30f;

        private void Update()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (isAmmo)
                {
                    other.GetComponent<AmmoInventory>().AddAmmo(ammoToAdd);
                    //GameObject vfxInstance = Instantiate(ammoVFX, transform.position, Quaternion.Identity);
                    //Destroy(vfxInstance, vfxInstance.GetComponent<ParticleSystem>().main.duration);
                    GameObject.FindGameObjectWithTag("GameSoundManager").GetComponent<AudioSource>().PlayOneShot(ammoSFX); 
                    //finding game objects with tags is fairly cheap and efficient, so I don't have to use a singleton here.
                    Destroy(gameObject); 
                }
                else if (isHealth) 
                {
                    other.GetComponent<Health>().AddHealth(healthToAdd);
                    Destroy(gameObject);
                }
                else if(isAmmo && isHealth) 
                {
                    other.GetComponent<Health>().AddHealth(healthToAdd);
                    other.GetComponent<AmmoInventory>().AddAmmo(ammoToAdd);
                    Destroy(gameObject);
                }
            }
        }
    }
}
