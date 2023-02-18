using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

namespace OperationPolygon.Combat 
{
    public class Weapon : MonoBehaviour
    {
        private int reloadAnimHash = Animator.StringToHash("ReloadAnim");

        [Header("Main Weapon Information")]
        [SerializeField] private string weaponName;
        [SerializeField] private WeaponClass weaponClass;

        [Header("Components")]
        [SerializeField] private Transform muzzlePoint; //the bullet spawns at this location.
        [SerializeField] private Projectile weaponProjectile; //the projectile used by the weapon.
        [SerializeField] private ThirdPersonShooterController shooter;
        [SerializeField] private AimTarget aimTarget;
        [Header("FX Components")]
        [SerializeField] private ParticleSystem muzzleFlashFX;
        [SerializeField] private ParticleSystem bulletEjectFX;
        [Header("Audio FX Components")]
        [SerializeField] private AudioSource weaponAudioSource;
        [SerializeField] private AudioClip[] weaponShotSounds;
        [SerializeField] private AudioClip weaponReloadSound;
        [SerializeField] private AudioClip weaponEmptySound;


        //input actions
        private Inputs input;
        private InputActions inputActions;
        private PlayerInput playerInput;

        [Header("Variables")]
        [SerializeField] private float fireRate; //rate of fire of the weapon
        [SerializeField] private float damageToDeal;
        [SerializeField] private int magSize = 30;
        //serialized for testing purposes
        [SerializeField]private int currentAmmoInMag;

        private bool isReloading = false;

        private void Awake()
        {
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<Inputs>();
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            aimTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<AimTarget>();
            shooter = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
            inputActions = new InputActions();

            inputActions.Player.Enable();
            inputActions.Player.Shoot.performed += OnShoot;
            inputActions.Player.Reload.performed += OnReload;
        }

        private void Start()
        {
            currentAmmoInMag = magSize;

        }

        private void Update()
        {

            //I'm aware that if statements are not the optimal way to this.
            //Instead I should be subscribing and unsubscribing to events.
            //TODO: Change the Input system functionality - 
            //Events instead of Messages. This will take a day or two, but I'm sure I can do it.
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (context.performed && shooter.IsAiming() && currentAmmoInMag > 0 && !isReloading)
            {
                Debug.Log(context);
                Vector3 muzzleDirection = (aimTarget.GetMouseWorldPosition() - muzzlePoint.position).normalized;
                Instantiate(weaponProjectile, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
                AudioClip clipToPlay = weaponShotSounds[Random.Range(0, weaponShotSounds.Length)];
                weaponAudioSource.PlayOneShot(clipToPlay);
                muzzleFlashFX.Play();
                bulletEjectFX.Play();
                currentAmmoInMag--;
            }
            else if(context.performed && shooter.IsAiming() && currentAmmoInMag == 0 && !isReloading) 
            {
                weaponAudioSource.PlayOneShot(weaponEmptySound);
            }
        }

        public void OnReload(InputAction.CallbackContext context) 
        {
            //maths for max ammo in reserve, and logic, etc will go here.
            //current ammo in backpack
            //current ammo in backpack -= currentAmmo in mag;
            //current ammo in backpack < magSize
            //currentAmmoInMag = current ammo in backpack
            //current Ammo in backpack == 0;
            //if(current ammo in backpack == 0) return;
            //the reload system is very simple (for now), will try to add animations and anim rigging later.
            //if(currentAmmoInMag == mag size) return;
            if (context.performed && currentAmmoInMag < magSize) 
            {
                isReloading = true;
                StartCoroutine(ReloadAnimationWaitTime());
            }
        }

        private IEnumerator ReloadAnimationWaitTime() 
        {
            shooter.GetAnimator().Play(reloadAnimHash, 0);
            if (shooter.IsAiming()) 
            {
                shooter.GetAnimator().Play(reloadAnimHash, 1);
            }
            yield return new WaitForSeconds(2f);
            currentAmmoInMag = magSize;
            isReloading = false;
        }



        //public getter classes

        public int GetCurrentAmmoInMag() 
        {
            return currentAmmoInMag;
        }

        //notes I left in ThirdPersonShooterController
        //shoot mechanic - refactor and edit later
        //this functionality is going into the weapon along with planned features such as
        //weapon spread
        //weapon damage
        //fire rate
        //mag size
        //weapon will also handle all FX related to it
        //projectile will trigger FX such as hit events and blood splatter.
    }
}
