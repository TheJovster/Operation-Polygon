using StarterAssets;
using UnityEngine;

namespace OperationPolygon.Combat 
{
    public class WeaponSpread : MonoBehaviour
    {
        Weapon weapon;
        Inputs input;
        ThirdPersonController controller;
        ThirdPersonShooterController shooterController;

        [SerializeField]private float currentAimSpreadAngle;
        [SerializeField] private float defaultSpreadAngle = 3f;
        [SerializeField]private float aimSpreadAngleMultiplier = .5f;
        [SerializeField]private float moveSpreadAngleMultiplier = 2f;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
            shooterController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonShooterController>();
            input = GameObject.FindGameObjectWithTag("Player").GetComponent<Inputs>();
        }

        public Vector3 SpreadAngle(Transform muzzlePos)
        {
            if (input.aim == true) 
            {
                currentAimSpreadAngle = defaultSpreadAngle * aimSpreadAngleMultiplier;
            }
            else if(input.aim != true) 
            {
                currentAimSpreadAngle = defaultSpreadAngle;
            }
            else if(input.move.magnitude != 0f) 
            {
                currentAimSpreadAngle = (defaultSpreadAngle * moveSpreadAngleMultiplier) * aimSpreadAngleMultiplier;
            }

            float randomXAngle = Random.Range(-currentAimSpreadAngle, currentAimSpreadAngle);
            float randomYAngle = Random.Range(-currentAimSpreadAngle, currentAimSpreadAngle);
            float randomZAngle = Random.Range(-currentAimSpreadAngle, currentAimSpreadAngle);

            Vector3 randomRotation = new Vector3(randomXAngle, randomYAngle, randomZAngle);
            return muzzlePos.localEulerAngles + randomRotation;

            //NOTE TO SELF, TODO
            //As the functionality and complexity expands, I'll have to add:
            //checks for crouching and movement
            //checks for crouching and aiming
            //checks for crouching and aiming and movement - I'll need variables for this
            //variables to add: crouchSpreadAngleMultiplier
        }

    }
}

