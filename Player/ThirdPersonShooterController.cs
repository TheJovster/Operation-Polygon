using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    //camera
    [SerializeField] private CinemachineVirtualCamera aimCamera;

    //components
    private StarterAssetsInputs inputs;
    private ThirdPersonController controller;

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
            canShoot = true;
            aimCamera.gameObject.SetActive(true);
            controller.SetMouseSensitivityFraction(mouseAimSensitivity);
            controller.SetRotationWithMovement(false);
            crosshairHip.gameObject.SetActive(false);
            crosshairAim.gameObject.SetActive(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * animLerpTime));
            Vector3 aimTarget = mouseWorldPosition;
            aimTarget.y = transform.position.y;
            Vector3 aimDirection = (aimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); //bad practice here
            if (inputs.shoot && canShoot)
            {
                Shoot(mouseWorldPosition);
            }
        }
        else
        {
            canShoot = false;
            aimCamera.gameObject.SetActive(false);
            controller.SetMouseSensitivityFraction(defaultMouseSensitivity);
            controller.SetRotationWithMovement(true);
            crosshairAim.gameObject.SetActive(false);
            crosshairHip.gameObject.SetActive(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * animLerpTime));
        }





    }

    private void Shoot(Vector3 mousePosition) 
    {
        Debug.Log("Pew pew");
        Vector3 muzzleDirection = (mousePosition - muzzlePoint.position).normalized;
        Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(muzzleDirection));
        inputs.shoot = false;
    }

}
