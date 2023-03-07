using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.AI
{
    public class EnemyWeapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int damage = 3;
        [SerializeField] private float fireRate = 1f;
        private float timeSinceLastShot = 0f;
        

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
        }

        public void Shoot()
        {
            if (timeSinceLastShot >= fireRate)
            {
                Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
                timeSinceLastShot = 0f;
                Debug.Log("Shot at player");
            }
        }

        public int GetDamage() 
        {
            return damage;
        }

    }
}
