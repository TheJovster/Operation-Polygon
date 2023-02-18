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
        private Inputs input;
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

        //serialized for testing
        [SerializeField]private bool isAiming = false;

        //LayerMask;
        [SerializeField] private LayerMask targetLayerMask = new LayerMask();

        //components in children

        void Awake()
        {
            input = GetComponent<Inputs>();
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

            if (input.aim)
            {
                
                aimCamera.gameObject.SetActive(true);
                controller.SetMouseSensitivityFraction(mouseAimSensitivity);
                controller.SetRotationWithMovement(false);
                aimRigWeight = 1f;
                isAiming = true;
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
                isAiming = false;
                crosshairAim.gameObject.SetActive(false);
                crosshairHip.gameObject.SetActive(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * animLerpTime));
            }
        }

        public bool IsAiming() 
        {
            return isAiming;
        }

        public Animator GetAnimator() 
        {
            return animator;
        }

/*        private void Shoot(Vector3 mousePosition) //add this to the weapon system
 *        Psuedocode left for reference
            //note to self - remove the pass-through Vector3. 
            //weapon namespace will have to rely heavily on the weapon system.
        {
            Debug.Log("Pew pew");
            Vector3 muzzleDirection = (mousePosition - muzzlePoint.position).normalized;
            Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
            inputs.shoot = false;
        }*/

        //animation events

        public void OnReloadStart() 
        {
            GetComponentInChildren<Weapon>().PlayReloadStartSound();
        }

        public void OnReloadMid() 
        {
            GetComponentInChildren<Weapon>().PlayReloadMidSound();
        }

        public void OnReloadEnd()
        {
            GetComponentInChildren<Weapon>().PlayReloadEndSound();
        }
    }
}
