using System;
using Unity.VisualScripting;
using UnityEngine;

namespace SCS.Scripts.GamePlay
{
    public class FollowCamera : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Target")]
        [SerializeField] private Transform _followTarget;
        [SerializeField] private float _followSpeed = 4.0f;

        [Space]
        [Header("Follow Axis")]
        [SerializeField] private bool _x;
        [SerializeField] private bool _y;
        [SerializeField] private bool _z;

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private void OnValidate()
        {
            if (_followTarget.IsUnityNull())
                throw new UnassignedReferenceException(_followTarget.name);
        }

        private void Awake()
        {
            // get follow target position
            var followTargetPosition = new Vector3(
                _x ? _followTarget.position.x : transform.position.x,
                _y ? _followTarget.position.y : transform.position.y,
                _z ? _followTarget.position.z : transform.position.z);

            // follow
            transform.position = followTargetPosition;
        }

        private void Update()
        {
            // get follow target position
            var followTargetPosition = new Vector3(
                _x ? _followTarget.position.x : transform.position.x,
                _y ? _followTarget.position.y : transform.position.y,
                _z ? _followTarget.position.z : transform.position.z);

            // follow
            transform.position = Vector3.Lerp(transform.position, followTargetPosition, _followSpeed * Time.deltaTime);
        }
    }
}
