using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;

namespace OperationPolygon.Combat 
{
    public class ThirdPersonShooterController : MonoBehaviour
    {
        //camera
        [SerializeField] private CinemachineVirtualCamera aimCamera;

        //components
        private StarterAssetsInputs inputs;
        private ThirdPersonController controller;
        [SerializeField] private Rig aimRig;

        //UIComponents
        [SerializeField] private Image crosshairHip;
        [SerializeField] private Image crosshairAim;
        private Animator animator;

        //variables
        [SerializeField] private float maximumAimDistance = 600f;
        [SerializeField] private float defaultMouseSensitivity = 1f;
        [SerializeField] private float mouseAimSensitivity = .333f;
        [SerializeField] private float aimMoveSpeed = 1f;
        [SerializeField] private float aimSprintSpeed = 2.65f;
        [SerializeField] private float animLerpTime = 10f;
        [SerializeField] private float rigWeightLerpTime = 20f;
        private float aimRigWeight;

        private bool canShoot = false;

        //LayerMask;
        [SerializeField] private LayerMask targetLayerMask = new LayerMask();

        //prefabs - weapon will handle this later
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private Transform bulletPrefab;

        void Awake()
        {
            inputs = GetComponent<StarterAssetsInputs>();
            controller = GetComponent<ThirdPersonController>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            AimState();
            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * rigWeightLerpTime);
        }

        private void AimState()
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenter = new Vector2(Screen.width * .5f, Screen.height * .5f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit hit, maximumAimDistance, targetLayerMask))
            {
                mouseWorldPosition = hit.point;
            }

            if (inputs.aim)
            {
                aimCamera.gameObject.SetActive(true);
                controller.SetMouseSensitivityFraction(mouseAimSensitivity);
                controller.SetRotationWithMovement(false);
                aimRigWeight = 1f;
                crosshairHip.gameObject.SetActive(false);
                crosshairAim.gameObject.SetActive(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * animLerpTime));
                Vector3 aimTarget = mouseWorldPosition;
                aimTarget.y = transform.position.y;
                Vector3 aimDirection = (aimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); //bad practice here

            }
            else
            {
                aimCamera.gameObject.SetActive(false);
                controller.SetMouseSensitivityFraction(defaultMouseSensitivity);
                controller.SetRotationWithMovement(true);
                aimRigWeight = 0f;
                crosshairAim.gameObject.SetActive(false);
                crosshairHip.gameObject.SetActive(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * animLerpTime));
            }



            //shoot mechanic - refactor and edit later
            //this functionality is going into the weapon along with planned features such as
            //weapon spread
            //weapon damage
            //fire rate
            //mag size
            //weapon will also handle all FX related to it
            //projectile will trigger FX such as hit events and blood splatter.
            if (inputs.shoot)
            {
                Shoot(mouseWorldPosition);
            }

        }
        private void Shoot(Vector3 mousePosition) //add this to the weapon system
            //note to self - remove the pass-through Vector3. 
            //weapon namespace will have to rely heavily on the weapon system.
        {
            Debug.Log("Pew pew");
            Vector3 muzzleDirection = (mousePosition - muzzlePoint.position).normalized;
            Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
            inputs.shoot = false;
        }

    }
}
