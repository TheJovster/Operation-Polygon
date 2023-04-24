using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private Weapon[] weaponsList; //I'm using a List because it's easier to expand at runtime than Arrays.
        [Header("The object that contains the weapon game objects. In this case, it's Hand_R game object in the PlayerArmature child object named Root.")]
        [SerializeField] private Transform weaponContainer;
        private Weapon currentWeapon;
        private int currentWeaponIndex;

        private void Awake()
        {
            

        }

        
    }
}
