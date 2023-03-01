using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoilHandler : MonoBehaviour
{
    private CinemachineImpulseSource source;

    private void Awake()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void TriggerRecoil() 
    {
        source.GenerateImpulse();
    }
    /*    //transform
     *    //commented out for testing purposes
        private Transform weaponRecoilPos;

        //variables
        [SerializeField] private float cameraKickAmount;
        [SerializeField] private float cameraKickSpeed;
        [SerializeField] private float normalizationSpeed;
        private float currentRecoilPos, finalRecoilPos;

        private void Awake()
        {
            weaponRecoilPos = GameObject.FindGameObjectWithTag("PlayerRecoilHandler").GetComponent<Transform>();
        }

        private void Update()
        {
            currentRecoilPos = Mathf.Lerp(currentRecoilPos, 0f, normalizationSpeed * Time.deltaTime);
            finalRecoilPos = Mathf.Lerp(finalRecoilPos, currentRecoilPos, cameraKickSpeed * Time.deltaTime);
            weaponRecoilPos.localPosition = new Vector3(0f, 0f, finalRecoilPos);
        }

        public void TriggerRecoil() 
        {
            currentRecoilPos += cameraKickAmount;
        }*/
}
