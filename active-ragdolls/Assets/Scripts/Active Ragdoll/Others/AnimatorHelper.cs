﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveRagdoll {
    /// <summary>
    /// Helper class that contains a lot of necessary functionality to control the animator,
    /// and more especifically the IK.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorHelper : MonoBehaviour {
        private Animator _animator;

        // Targets for the IK
        private Transform _targetsParent;
        private Transform _lookTarget;
        private Vector3 _lookPoint = Vector3.zero;
        private bool _lookingAtPoint = true; // It can be either looking at a target or at a point
        private Transform _leftHandTarget, _rightHandTarget, _leftHandHint, _rightHandHint;
        private Transform _leftFootTarget, _rightFootTarget;

        // These weights define how much influence the IK will have over the animation
        private float _leftArmIKWeight = 0, _rightArmIKWeight = 0;
        private float _leftLegIKWeight = 0, _rightLegIKWeight = 0;

        // How much the chest IK will influence the animation at its maximum
        private float _chestMaxLookWeight = 0.5f;

        // Transitions stop body extremities to 'snap' after quickly going from
        // IK to animation. If the player stops pressing the arm button, for example,
        // the arm will snap right back into the animation position if not smoothed.
        private bool _smoothIKTransition = true;
        private float _ikTransitionSpeed = 10;

        // Values used for animating the transition between animation & IK
        private float _currentLeftArmIKWeight = 0, _currentRightArmIKWeight = 0;
        private float _currentLeftLegIKWeight = 0, _currentRightLegIKWeight = 0;

        void Awake() {
            _animator = GetComponent<Animator>();

            _targetsParent = new GameObject("IK Targets").transform;
            _targetsParent.parent = transform.parent;

            _leftHandTarget = new GameObject("LeftHandTarget").transform;
            _rightHandTarget = new GameObject("RightHandTarget").transform;
            _leftHandTarget.parent = _targetsParent;
            _rightHandTarget.parent = _targetsParent;

            _leftHandHint = new GameObject("LeftHandHint").transform;
            _rightHandHint = new GameObject("RightHandHint").transform;
            _leftHandHint.parent = _leftHandTarget;
            _rightHandHint.parent = _rightHandTarget;
            _leftHandHint.Translate(new Vector3(-1, -1, 0), Space.Self);
            _rightHandHint.Translate(new Vector3(1, -1, 0), Space.Self);

            _leftFootTarget = new GameObject("LeftFootTarget").transform;
            _rightFootTarget = new GameObject("RightFootTarget").transform;
            _leftFootTarget.parent = _targetsParent;
            _rightFootTarget.parent = _targetsParent;
        }

        private void Update() {
            UpdateIKTransitions();
        }

        /// <summary>
        /// Updates the IK weights to smoothly transition between IK and animations.
        /// </summary>
        private void UpdateIKTransitions() {
            _currentLeftArmIKWeight = Mathf.Lerp(_currentLeftArmIKWeight, _leftArmIKWeight, Time.deltaTime * _ikTransitionSpeed);
            _currentLeftLegIKWeight = Mathf.Lerp(_currentLeftLegIKWeight, _leftLegIKWeight, Time.deltaTime * _ikTransitionSpeed);
            _currentRightArmIKWeight = Mathf.Lerp(_currentRightArmIKWeight, _rightArmIKWeight, Time.deltaTime * _ikTransitionSpeed);
            _currentRightLegIKWeight = Mathf.Lerp(_currentRightLegIKWeight, _rightLegIKWeight, Time.deltaTime * _ikTransitionSpeed);
        }

        // Tells the Animator which weight, position and rotation values to use each frame/
        private void OnAnimatorIK(int layerIndex) {
            // Look
            _animator.SetLookAtWeight(1, ((_leftArmIKWeight + _rightArmIKWeight) / 2) * _chestMaxLookWeight, 1, 1, 0);
            _animator.SetLookAtPosition(_lookingAtPoint? _lookPoint : _lookTarget.position);

            // Left arm
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _currentLeftArmIKWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _currentLeftArmIKWeight);
            _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, _leftArmIKWeight);

            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandTarget.rotation);
            _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _leftHandHint.position);

            // Right arm
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _currentRightArmIKWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _currentRightArmIKWeight);
            _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, _rightArmIKWeight);

            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandTarget.rotation);
            _animator.SetIKHintPosition(AvatarIKHint.RightElbow, _rightHandHint.position);

            // Left leg
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _currentLeftLegIKWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _currentLeftLegIKWeight);

            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFootTarget.rotation);

            // Right leg
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _currentRightLegIKWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _currentRightLegIKWeight);

            _animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, _rightFootTarget.rotation);
        }


        // ------------------- GETTERS & SETTERS -------------------

        public Transform GetLookTarget() {
            return _lookTarget;
        }

        public Vector3 GetLookPoint() {
            return _lookPoint;
        }

        public void LookAtTarget(Transform target) {
            _lookTarget = target;
            _lookingAtPoint = false;
        }

        public void LookAtPoint(Vector3 point) {
            _lookPoint = point;
            _lookingAtPoint = true;
        }

        public Transform GetLeftHandTarget() {
            return _leftHandTarget;
        }
        public Transform GetRightHandTarget() {
            return _rightHandTarget;
        }
        public Transform GetLeftFootTarget() {
            return _leftFootTarget;
        }
        public Transform GetRightFootTarget() {
            return _rightFootTarget;
        }

        public void SetLeftArmIKWeight(float weight) {
            _leftArmIKWeight = weight;
        }

        public void SetRightArmIKWeight(float weight) {
            _rightArmIKWeight = weight;
        }
        public void SetLeftLegIKWeight(float weight) {
            _leftLegIKWeight = weight;
        }

        public void SetRightLegIKWeight(float weight) {
            _rightLegIKWeight = weight;
        }

        public void SetChestMaxLookWeight(float chestMaxLookWeight) {
            _chestMaxLookWeight = chestMaxLookWeight;
        }

        public float GetLeftArmIKWeight() {
            return _leftArmIKWeight;
        }

        public float GetRightArmIKWeight() {
            return _rightArmIKWeight;
        }

        public float GetLeftLegIKWeight() {
            return _leftLegIKWeight;
        }

        public float GetRightLegIKWeight() {
            return _rightLegIKWeight;
        }

        public void SetSmoothIKTransition(bool smoothIKTransition) {
            _smoothIKTransition = smoothIKTransition;
        }

        public bool GetSmoothIKTransition() {
            return _smoothIKTransition;
        }

        public void SetIKTransitionSpeed(float speed) {
            _ikTransitionSpeed = speed;
        }

        public float GetIKTransitionDuration() {
            return _ikTransitionSpeed;
        }
    }
} // namespace ActiveRagdoll