using System.Collections;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System;

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
        private WeaponRecoilHandler recoilHandler;
        private WeaponSpread spreadHandler;

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
        private InputActions inputActions;
        private PlayerInput playerInput;

        [Header("Variables")]
        [SerializeField] private bool isSemi = false;
        [SerializeField] private float fireRate; //rate of fire of the weapon
        private float fireRateWhenEmpty = .5f; //this is going to slow down the playing of the PlayEmptyAndReturn method.
        private float timeSinceLastShot;
        [SerializeField] private float damageToDeal;
        [SerializeField] private int magSize = 30;
        //serialized for testing purposes
        [SerializeField] private int currentAmmoInMag;

        private bool isReloading = false;

        private float animLerpTime = 10f; //using a larger value because I'm using this value for a Lerp function
        private float animTransitionTime = .25f; //using this value for Animator transition time.

        private void Awake()
        {
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<Inputs>();
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            aimTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<AimTarget>();
            shooter = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
            recoilHandler = GetComponent<WeaponRecoilHandler>();
            spreadHandler = GetComponent<WeaponSpread>();
            inputActions = new InputActions();

            inputActions.Player.Enable();
            inputActions.Player.Shoot.performed += OnShoot;
            inputActions.Player.Reload.performed += OnReload;
            inputActions.Player.Reload.canceled -= OnReload;
        }

        private void Start()
        {

            currentAmmoInMag = magSize;
            timeSinceLastShot = fireRate;

        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime; //this is a constant timer that keeps updating timeSinceLastShot.

            if (!isSemi && input.shoot)
            {
                AutomaticFire();
            }

            if (!input.shoot) 
            {
                ResetMuzzlePointRotation();
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

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (isSemi)
            {
                if (shooter.IsAiming() && currentAmmoInMag > 0 && !isReloading)
                {
                    if (context.performed && currentAmmoInMag > 0 && timeSinceLastShot >= fireRate) //for some reason, this was the only way I found to make the weapon fire automatically
                    {
                        ShootAction();
                    }
                }
                if (shooter.IsAiming() && currentAmmoInMag == 0 && !isReloading)
                {
                    PlayWeaponEmptyAndReturn();
                }
            }
        }

        private void ShootAction() //shoot action contains all of the logic for shooting.
        {
            timeSinceLastShot = 0;
            Vector3 muzzleDirection = (aimTarget.GetMouseWorldPosition() - muzzlePoint.position).normalized;
            muzzlePoint.localEulerAngles = spreadHandler.SpreadAngle(muzzlePoint);
            Instantiate(weaponProjectile, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
            recoilHandler.TriggerRecoil();
            AudioClip clipToPlay = weaponShotSounds[UnityEngine.Random.Range(0, weaponShotSounds.Length)]; //why am I being explicit? Because the code demands it.
            weaponAudioSource.PlayOneShot(clipToPlay);
            muzzleFlashFX.Play();
            bulletEjectFX.Play();
            currentAmmoInMag--;
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
            shooter.GetAnimator().SetLayerWeight(2, 1f);
            if (shooter.IsAiming())
            {
                shooter.GetAnimator().Play(reloadAnimHash, 2, animTransitionTime);
            }
            shooter.GetAnimator().Play(reloadAnimHash, 2, animTransitionTime);
            yield return new WaitForSeconds(1.6f);
            currentAmmoInMag = magSize;
            shooter.GetAnimator().SetLayerWeight(2, 0f);
            isReloading = false;
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
