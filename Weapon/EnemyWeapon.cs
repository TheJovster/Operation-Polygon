using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.AI
{
    public class EnemyWeapon : MonoBehaviour
    {
        [SerializeField] private AudioSource muzzleAudioSource;
        [SerializeField] private AudioClip[] gunShotClips;
        [SerializeField] private ParticleSystem gunShotParticle;

        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int damage = 3;
        [SerializeField] private float fireRate = 1f;
        private float timeSinceLastShot = 0f;
        private Health health;

        private void Awake()
        {
            health = GetComponentInParent<Health>();
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
        }

        public void Shoot()
        {
            if (health.IsAlive())
            {
                if (timeSinceLastShot >= fireRate)
                {
                    int randomClipIndex = Random.Range(0, gunShotClips.Length);
                    muzzleAudioSource.PlayOneShot(gunShotClips[randomClipIndex]);
                    gunShotParticle.Play();
                    Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
                    timeSinceLastShot = 0f;
                    Debug.Log("Shot at player");
                }
            }

        }

        public int GetDamage() 
        {
            return damage;
        }

    }
}
