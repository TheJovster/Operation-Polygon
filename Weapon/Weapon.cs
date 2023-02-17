using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Windows;
using UnityEngine.Timeline;

namespace OperationPolygon.Combat 
{
    public class Weapon : MonoBehaviour
    {
        private int reloadAnimHash = Animator.StringToHash("ReloadAnim");

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
        [SerializeField] private AudioClip weaponShotSound;
        [SerializeField] private AudioClip weaponReloadSound;

        

        private StarterAssetsInputs input;

        [Header("Variables")]
        [SerializeField] private float fireRate; //rate of fire of the weapon
        [SerializeField] private float damageToDeal;
        [SerializeField] private int magSize = 30;
        //serialized for testing purposes
        [SerializeField]private int currentAmmoInMag;

        private bool isReloading = false;

        private void Awake()
        {
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
            aimTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<AimTarget>();
            shooter = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
        }

        private void Start()
        {
            currentAmmoInMag = magSize;
        }

        private void Update()
        {
            if(input.shoot && shooter.IsAiming() && currentAmmoInMag > 0 && !isReloading) 
            {
                Shoot();
            }
            if (input.reload) 
            {
                Reload();
            }

            //I'm aware that if statements are not the optimal way to this.
            //Instead I should be subscribing and unsubscribing to events.
            //TODO: Change the Input system functionality - 
            //Events instead of Messages. This will take a day or two, but I'm sure I can do it.
        }

        private void Shoot()
        {
            Debug.Log("Pew pew");
            Vector3 muzzleDirection = (aimTarget.GetMouseWorldPosition() - muzzlePoint.position).normalized;
            Instantiate(weaponProjectile, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
            muzzleFlashFX.Play();
            bulletEjectFX.Play();
            input.shoot = false;
            currentAmmoInMag--;
            if(currentAmmoInMag == 0) 
            {
                Reload();
            }
        }
        
        private void Reload() 
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
            isReloading = true;
            StartCoroutine(ReloadAnimationWaitTime());
            Debug.Log(currentAmmoInMag);
            input.reload = false;
        }

        private IEnumerator ReloadAnimationWaitTime() 
        {
            shooter.GetAnimator().Play(reloadAnimHash, 0);
            if (shooter.IsAiming()) 
            {
                shooter.GetAnimator().Play(reloadAnimHash, 1);
            }
            yield return new WaitForSeconds(shooter.GetAnimator().GetCurrentAnimatorClipInfo(0).Length); 
            //I was being dumb - I had already set Reload anim as the current animation,
            //therefore it's the current clip.
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
