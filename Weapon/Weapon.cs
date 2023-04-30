using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System;
using Unity.VisualScripting;

namespace OperationPolygon.Combat 
{
    [HideInInspector]
    public enum WeaponClass
    {
        AssaultRifle,
        SMG,
        LMG,
        SniperRifle,
        Launcher
    }
    
    public class Weapon : MonoBehaviour
    {
        private int reloadAnimHash = Animator.StringToHash("ReloadAnim");

        [Header("Main Weapon Information & Overrides")]
        [SerializeField] private AnimatorOverrideController animOverride;
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public WeaponClass WeaponClass { get; private set; }
        private AmmoInventory ammoInventory;
        private WeaponInventory weaponInventory;

        [Header("Components")]
        [SerializeField] private Transform muzzlePoint; //the bullet spawns at this location.
        [SerializeField] private Projectile weaponProjectile; //the projectile used by the weapon.
        [SerializeField] private ThirdPersonShooterController shooter;
        [SerializeField] private AimTarget aimTarget;
        private WeaponRecoilHandler recoilHandler;
        private WeaponSpread spreadHandler;
        private UIManager uiManager;

        [Header("FX Components")]
        [SerializeField] private ParticleSystem muzzleFlashFX;
        [SerializeField] private ParticleSystem bulletEjectFX;
        [Header("Audio FX Components")]
        [SerializeField] private AudioSource weaponAudioSource;
        [SerializeField] private AudioClip[] weaponShotSounds;
        [SerializeField] private AudioClip[] weaponReloadSounds;
        [SerializeField] private AudioClip weaponEmptySound;


        //input actions
        private Inputs input;
        private InputHandler inputActions;
        private PlayerInput playerInput;

        [Header("Small Arms Variables")]
        [SerializeField] private bool hasSelectFire;
        [field: SerializeField] public bool IsSemi { get; private set; }

        [SerializeField] private float fireRate; //rate of fire of the weapon
        private float fireRateWhenEmpty = .5f; //this is going to slow down the playing of the PlayEmptyAndReturn method.
        private float timeSinceLastShot;
        [SerializeField] private int weaponDamage;
        [SerializeField] private int magSize = 30;
        //serialized for testing purposes
        [SerializeField] private int currentAmmoInMag;
        [Header("Launcher variables")]
        [SerializeField] private GameObject grenadePrefab;
        [field: SerializeField] public bool IsLauncher { get; private set; }
        [SerializeField] private float launchForce = 10f;
        [SerializeField, Range(15f, 75f)] private float launchAngle = 45f;

        [Header("Flashlight variables and components")]
        [SerializeField] private Light flashLightSource;
        private bool flashLightOn = false;

        private bool isReloading = false;

        //private float animLerpTime = 10f; //using a larger value because I'm using this value for a Lerp function
        private float animTransitionTime = .25f; //using this value for Animator transition time.



        private void Awake()
        {
            input = GetComponentInParent<Inputs>();
            playerInput = GetComponentInParent<PlayerInput>();
            aimTarget = GetComponentInParent<AimTarget>();
            shooter = GetComponentInParent<ThirdPersonShooterController>();
            ammoInventory = GetComponentInParent<AmmoInventory>();
            weaponInventory = GetComponentInParent<WeaponInventory>();
            recoilHandler = GetComponent<WeaponRecoilHandler>();
            spreadHandler = GetComponent<WeaponSpread>();
            uiManager = GetComponentInParent<UIManager>();
            inputActions = new InputHandler();

            inputActions.Player.Enable();
            inputActions.Player.Shoot.performed += OnShoot;
            inputActions.Player.Reload.performed += OnReload;
            inputActions.Player.Reload.canceled -= OnReload;
            if (flashLightSource != null && !flashLightOn) 
            {
                flashLightSource.enabled = false;
            }
            //weaponInventory.SetCurrentWeapon(this);
        }

        private void OnEnable()
        {
            weaponInventory.SetCurrentWeapon(this);
            ammoInventory.InitializeCurrentWeaponData();
            uiManager.InitalizeUI();
            isReloading = false;
            timeSinceLastShot = fireRate;
            shooter.GetAnimator().runtimeAnimatorController = animOverride;
        }

        private void OnDisable()
        {
            isReloading = true;
            timeSinceLastShot = 0;
        }

        private void Start()
        {
            currentAmmoInMag = magSize;
            timeSinceLastShot = fireRate;
            if (animOverride != null)
            {
                shooter.GetAnimator().runtimeAnimatorController = animOverride;
            }
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime; //this is a constant timer that keeps updating timeSinceLastShot.

            if (!IsSemi && input.shoot && shooter.IsAiming()) //automatic fire has to be setup like this because there needs to be a constant check if input.shoot is held && a constant check to see if timeSinceLastTime >= fireRate
            {
                AutomaticFire();
            }

            if (!input.shoot) 
            {
                ResetMuzzlePointRotation();
            }
            if (Input.GetKeyDown(KeyCode.L)) 
            {
                ToggleFlashlight();
            }
            if (Input.GetKeyDown(KeyCode.X) && hasSelectFire)
            {
                SelectFireMode();
            }
        }



        private void PlayWeaponEmptyAndReturn()
        {
            weaponAudioSource.PlayOneShot(weaponEmptySound);
            timeSinceLastShot = 0f;
            ResetMuzzlePointRotation();
            return;
        }

        private void ResetMuzzlePointRotation()
        {
            muzzlePoint.localEulerAngles = Vector3.zero;
        }

        private void OnShoot(InputAction.CallbackContext context) //semi automatic fire.
        {
            if (IsSemi)
            {
                if (shooter.IsAiming() && currentAmmoInMag > 0 && context.performed && Input.GetMouseButtonDown(0))
                {

                    if ( currentAmmoInMag > 0 && timeSinceLastShot >= fireRate && !isReloading) //for some reason, this was the only way I found to make the weapon fire automatically
                    {
                        ShootAction();
                    }

                }
                if (shooter.IsAiming() && currentAmmoInMag == 0 && !isReloading && ammoInventory.GetCurrentAmmoInInventory() > 0)
                {
                    PlayWeaponEmptyAndReturn();
                    ShootAction();
                }
                else if(shooter.IsAiming() && currentAmmoInMag == 0 && !isReloading && ammoInventory.GetCurrentAmmoInInventory() <= 0) 
                {
                    PlayWeaponEmptyAndReturn();
                }
            }
        }

        private void AutomaticFire()
        {
            if (currentAmmoInMag > 0 && timeSinceLastShot >= fireRate)
            {
                ShootAction();
            }
            else if (currentAmmoInMag <= 0 && timeSinceLastShot >= fireRateWhenEmpty)
            {
                PlayWeaponEmptyAndReturn();
            }
        }

        private void ShootAction() //shoot action contains all of the logic for shooting.
        {
            if (!isReloading && !IsLauncher)  
            {
                timeSinceLastShot = 0;
                Vector3 muzzleDirection = (aimTarget.GetMouseWorldPosition() - muzzlePoint.position).normalized; //needs further testing
                muzzlePoint.localEulerAngles = spreadHandler.SpreadAngle(muzzlePoint); //needs further testing
                Instantiate(weaponProjectile, muzzlePoint.position, Quaternion.LookRotation(muzzlePoint.forward)); //needs further testing
                recoilHandler.TriggerRecoil();
                AudioClip clipToPlay = weaponShotSounds[UnityEngine.Random.Range(0, weaponShotSounds.Length)]; //why am I being explicit? Because the code demands it.
                weaponAudioSource.PlayOneShot(clipToPlay);
                muzzleFlashFX.Play();
                bulletEjectFX.Play();
                currentAmmoInMag--;
                if (ammoInventory.GetCurrentAmmoInInventory() != 0 && currentAmmoInMag <= 0)
                {
                    isReloading = true;
                    StartCoroutine(ReloadAnimationWaitTime());

                }
                
            }
            else if(!isReloading && IsLauncher) 
            {
                if(currentAmmoInMag > 0)
                LaunchProjectile();
                if (ammoInventory.GetCurrentAmmoInInventory() != 0 && currentAmmoInMag <= 0)
                {
                    isReloading = true;
                    StartCoroutine(ReloadAnimationWaitTime());
                }
            }
        }

        private void LaunchProjectile() //this is made for the shoot action
        {
            timeSinceLastShot = 0;
            weaponAudioSource.PlayOneShot(weaponShotSounds[0]);
            GameObject projectile = Instantiate(grenadePrefab, muzzlePoint.position, Quaternion.LookRotation(muzzlePoint.forward));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            float radianAngle = launchAngle * Mathf.Deg2Rad;
            Vector3 launchDirection = muzzlePoint.forward * Mathf.Cos(radianAngle) + muzzlePoint.up * Mathf.Sin(radianAngle);
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            currentAmmoInMag--;

            //TODO: Create ExplosiveProjectileClass
            //the projectile, upon hitting the ground is supposed to do:
            //coroutine
            //do a sphere cast and find all of the objects tagged enemy caught in the cast.
            //take away all of the health - do 200f damage;
            //wait .05 seconds
            //take all of the rigid bodies and add relative force to them, thus making the fly away
            //destroy game object after .5f
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed && currentAmmoInMag < magSize && ammoInventory.GetCurrentAmmoInInventory() > 0 && this.gameObject.activeInHierarchy)
            {
                isReloading = true;
                StartCoroutine(ReloadAnimationWaitTime());
            }
            else return;
        }

        private IEnumerator ReloadAnimationWaitTime()
        {
            shooter.GetAnimator().SetLayerWeight(2, 1f);
            shooter.GetAnimator().SetLayerWeight(3, 0f);
            if (shooter.IsAiming())
            {
                shooter.GetAnimator().Play(reloadAnimHash, 2, animTransitionTime);
                shooter.GetAnimator().Play(reloadAnimHash, 3, animTransitionTime);
            }
            shooter.GetAnimator().Play(reloadAnimHash, 2, animTransitionTime);
            shooter.GetAnimator().Play(reloadAnimHash, 3, animTransitionTime);
            yield return new WaitForSeconds(1.6f);
            AmmoRemovalCalculation();
            shooter.GetAnimator().SetLayerWeight(2, 0f);
            shooter.GetAnimator().SetLayerWeight(3, 1f);
            isReloading = false;
        }

        //in reality this is a simple arithmetic + conditionals
        //could do this with a switch statement too, but dammit, I hate switch statements.
        private void AmmoRemovalCalculation() 
        {
            int ammoToRemove = magSize - currentAmmoInMag;
            if (ammoInventory.GetCurrentAmmoInInventory() >= magSize)
            {
                currentAmmoInMag = magSize;
            }
            else if (ammoInventory.GetCurrentAmmoInInventory() < magSize)
            {
                if (currentAmmoInMag + ammoInventory.GetCurrentAmmoInInventory() > magSize ||
                    currentAmmoInMag + ammoInventory.GetCurrentAmmoInInventory() == magSize)
                {
                    int conditionalAmmo_1 = magSize - currentAmmoInMag;
                    ammoInventory.RemoveAmmo(conditionalAmmo_1);
                    currentAmmoInMag = magSize;
                    return;
                }
                else if(currentAmmoInMag + ammoInventory.GetCurrentAmmoInInventory() < magSize) 
                {
                    int conditionalAmmo_2 = currentAmmoInMag;
                    currentAmmoInMag = conditionalAmmo_2 + ammoInventory.GetCurrentAmmoInInventory();
                    ammoInventory.RemoveAmmo(ammoInventory.GetCurrentAmmoInInventory());
                    return;
                }
                currentAmmoInMag = ammoInventory.GetCurrentAmmoInInventory();
            }
            ammoInventory.RemoveAmmo(ammoToRemove);
        }

        private void SelectFireMode()
        {
            IsSemi = !IsSemi;
            weaponAudioSource.PlayOneShot(weaponReloadSounds[0]);
        }

        private void ToggleFlashlight() 
        {
            if(flashLightSource != null) 
            {
                flashLightOn = !flashLightOn;
                if (flashLightOn)
                {
                    flashLightSource.enabled = true;
                }
                else if (!flashLightOn)
                {
                    flashLightSource.enabled = false;
                }
            }

        }

        //public getter classes

        public int GetCurrentAmmoInMag()
        {
            return currentAmmoInMag;
        }

        public float GetFireRate()
        {
            return fireRate;
        }

        public int GetWeaponDamage() 
        {
            return weaponDamage;
        }

        public Transform GetMuzzlePosition() 
        {
            return muzzlePoint;
        }

        //shoot mechanic - refactor and edit later
        //this functionality is going into the weapon along with planned features such as
        //weapon spread
        //weapon damage
        //fire rate
        //mag size
        //weapon will also handle all FX related to it
        //projectile will trigger FX such as hit events and blood splatter.

        //anim events - magic numbers are unavoidable here for now
        public void PlayReloadStartSound() 
        {
            weaponAudioSource.PlayOneShot(weaponReloadSounds[0]);
        }

        public void PlayReloadMidSound() 
        {
            weaponAudioSource.PlayOneShot(weaponReloadSounds[1]);
        }

        public void PlayReloadEndSound() 
        {
            weaponAudioSource.PlayOneShot(weaponReloadSounds[2]);
        }
    }
}
