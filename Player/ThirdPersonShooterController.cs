using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Transform debugTransform;

    //camera
    [SerializeField] private CinemachineVirtualCamera aimCamera;

    //components
    private StarterAssetsInputs inputs;
    private ThirdPersonController controller;

    //UIComponents
    [SerializeField] private Image crosshairHip;
    [SerializeField] private Image crosshairAim;

    //variables
    [SerializeField] private float maximumAimDistance = 600f;
    [SerializeField] private float defaultMouseSensitivity = 1f;
    [SerializeField] private float mouseAimSensitivity = .333f;
    [SerializeField] private float aimMoveSpeed = 1f;
    [SerializeField] private float aimSprintSpeed = 2.65f;

    //LayerMask;
    [SerializeField] private LayerMask targetLayerMask = new LayerMask();

    void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
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
            debugTransform.position = hit.point;
        }

        if (inputs.aim)
        {
            aimCamera.gameObject.SetActive(true);
            controller.SetMouseSensitivityFraction(mouseAimSensitivity);
            controller.SetMoveSpeed(aimMoveSpeed);
            controller.SetSprintSpeed(aimSprintSpeed);
            crosshairHip.gameObject.SetActive(false);
            crosshairAim.gameObject.SetActive(true);

            Vector3 aimTarget = mouseWorldPosition;
            aimTarget.y = transform.position.y;
            Vector3 aimDirection = aimTarget - transform.position.normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); //bad practice here
        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            controller.SetMouseSensitivityFraction(defaultMouseSensitivity);
            controller.ResetMoveSpeedValue();
            controller.ResetSprintSpeedValue();
            crosshairAim.gameObject.SetActive(false);
            crosshairHip.gameObject.SetActive(true);
        }


    }

}
