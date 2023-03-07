using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationPolygon.AI 
{
    [System.Serializable]
    public class HumanBone 
    {
        public HumanBodyBones bone;
        public float weight = 1f;
    }

    public class AIWeaponIK : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform aimTransform;

        [SerializeField] private int iteration = 10;

        [Range(0, 1)]
        [SerializeField] private float weight = 1.0f;

        [SerializeField] private float angleLimit = 90.0f;
        [SerializeField] private float distanceLimit = 1.5f;
        [SerializeField] private Vector3 aimOffset;

        [SerializeField] private HumanBone[] humanBones;
        private Transform[] boneTransforms;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
            boneTransforms = new Transform[humanBones.Length];

            for (int i = 0; i < boneTransforms.Length; i++)
            {
                boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
            }
        }

        private void LateUpdate()
        {
            if(aimTransform == null) 
            {
                return;
            }
            if(targetTransform == null) 
            {
                return;
            }

            Vector3 targetPosition = GetTargetPosition();
            for (int i = 0; i < iteration; i++)
            {
                for (int b = 0; b < boneTransforms.Length; b++)
                {
                    Transform bone = boneTransforms[b];
                    float boneWeight = humanBones[b].weight * weight;
                    AimAtTarget(bone, targetPosition, boneWeight);
                }

            }
        }

        private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
        {
            Vector3 aimDirection = aimTransform.forward;
            Vector3 targetDirection = (targetPosition + aimOffset) - aimTransform.position;
            Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
            Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
            bone.rotation = blendedRotation * bone.rotation;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 targetDirection = targetTransform.position - aimTransform.position;
            Vector3 aimDirection = aimTransform.forward;

            float blendOut = 0.0f;

            float targetAngle = Vector3.Angle(targetDirection, aimDirection);
            if (targetAngle > angleLimit)
            {
                blendOut += (targetAngle - angleLimit / 50.0f);
            }

            float targetDistance = targetDirection.magnitude;

            if (targetDistance < distanceLimit)
            {
                blendOut += distanceLimit - targetDistance;
            }

            Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
            return aimTransform.position + direction;
        }

        //public functions

        public void SetTargetTransform(Transform target)
        {
            targetTransform = target;
        }

        public void SetAimTransform(Transform aim) 
        {
            aimTransform = aim;
        }
    }
}
