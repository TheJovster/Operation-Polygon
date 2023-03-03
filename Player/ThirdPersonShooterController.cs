using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine;
using StarterAssets;
using OperationPolygon.Core;
using UnityEngine.UI;
using System;

namespace OperationPolygon.Combat 
{
    public class ThirdPersonShooterController : MonoBehaviour
    {
        //camera
        [SerializeField] private CinemachineVirtualCamera aimCamera;

        //components
        private Inputs input;
        private InputHandler inputHandler;
        private Health health;
        private ThirdPersonController controller;
        [SerializeField] private Rig aimRig;
        private GameManager gameManager;

        //UIComponents
        [SerializeField] private Image crosshairHip;
        [SerializeField] private Image crosshairAim;
        private Animator animator;

        //variables
        [SerializeField] private float maximumAimDistance = 600f;
        [SerializeField] private float defaultMouseSensitivity = 1f;
        [SerializeField] private float mouseAimSensitivity = .333f;
        /*        [SerializeField] private float aimMoveSpeed = 1f;
                [SerializeField] private float aimSprintSpeed = 2.65f;*/
        [SerializeField] private float animLerpTime = 10f;
        [SerializeField] private float rigWeightLerpTime = 20f;
        private float aimRigWeight;
        private bool weaponEquipped = true;

        //serialized for testing
        [SerializeField] private bool isAiming = false;
        [SerializeField] private bool shouldersSwapped = false;

        //LayerMask;
        [SerializeField] private LayerMask targetLayerMask = new LayerMask();

        //Animator Weight Blend Layer indices
        private int baseLayerIndex = 1;
        private int upperBodyLayerindex = 3;

        //animator Weight Blend Layer float
        private float weightOff = 0f;
        private float weightOn = 1f;

        private float lookForwardLerpValue = 20f;

        void Awake()
        {

            gameManager = FindObjectOfType<GameManager>();
            animator = GetComponent<Animator>();
            input = GetComponent<Inputs>();
            controller = GetComponent<ThirdPersonController>();
            health = GetComponent<Health>();
        }

        private void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            AimState();
            if (input.aim && input.switchShoulders && Time.timeScale == 1)
            {
                ShoulderSwitch();
            }
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

            if (input.aim && health.IsAlive() && Time.timeScale == 1)
            {

                aimCamera.gameObject.SetActive(true);
                controller.SetMouseSensitivityFraction(mouseAimSensitivity);
                controller.SetRotationWithMovement(false);
                aimRigWeight = weightOn;
                isAiming = true;
                crosshairHip.gameObject.SetActive(false);
                crosshairAim.gameObject.SetActive(true);
                animator.SetLayerWeight(baseLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseLayerIndex), weightOn, Time.deltaTime * animLerpTime));
                animator.SetLayerWeight(upperBodyLayerindex, Mathf.Lerp(animator.GetLayerWeight(upperBodyLayerindex), weightOff, Time.deltaTime * animLerpTime));
                Vector3 aimTarget = mouseWorldPosition;
                aimTarget.y = transform.position.y;
                Vector3 aimDirection = (aimTarget - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * lookForwardLerpValue); //bad practice here
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
                animator.SetLayerWeight(baseLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseLayerIndex), weightOff, Time.deltaTime * animLerpTime));
                animator.SetLayerWeight(upperBodyLayerindex, Mathf.Lerp(animator.GetLayerWeight(upperBodyLayerindex), weightOn, Time.deltaTime * animLerpTime));
            }
        }
        //why add crouch here? Because the ThirdPersonController doesn't have an animator reference on it.
        //it would be more 

        private void ShoulderSwitch()
        {

            shouldersSwapped = !shouldersSwapped;
            Cinemachine3rdPersonFollow followComponent = aimCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            if (!shouldersSwapped)
            {
                followComponent.CameraSide = Mathf.Lerp(followComponent.CameraSide, 1f, 10f); //magic number - make exposed variables later
                input.switchShoulders = false;
            }
            else if (shouldersSwapped)
            {
                followComponent.CameraSide = Mathf.Lerp(followComponent.CameraSide, 0f, 10f);
                input.switchShoulders = false;
            }

        }

        //public getters

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
