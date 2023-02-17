using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Windows;

namespace OperationPolygon.Combat 
{
    public class Weapon : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform muzzlePoint; //the bullet spawns at this location.
        [SerializeField] private Projectile weaponProjectile; //the projectile used by the weapon.
        [SerializeField] private ThirdPersonShooterController shooter;
        [SerializeField] private AimTarget aimTarget;

        private StarterAssetsInputs input;

        [Header("Variables")]
        [SerializeField] private float fireRate; //rate of fire of the weapon
        [SerializeField] private float damageToDeal;

        private void Awake()
        {
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssetsInputs>();
            aimTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<AimTarget>();
            shooter = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
        }

        private void Update()
        {
            if (input.shoot && shooter.IsAiming())
            {
                Shoot();
            }
            else return;
        }

        private void Shoot()
        {
            Debug.Log("Pew pew");
            Vector3 muzzleDirection = (aimTarget.GetMouseWorldPosition() - muzzlePoint.position).normalized;
            Instantiate(weaponProjectile, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
            input.shoot = false;
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
