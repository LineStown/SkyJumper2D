using Unity.VisualScripting;
using UnityEngine;

namespace SCS.Scripts.GamePlay.Platforms
{
    public class RotatePlatform : BasePlatform
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Rotate Speed")]
        [SerializeField] private float _minSpeed = 0.0f;
        [SerializeField] private float _maxSpeed = 45.0f;
        private float _speed;
        private int _direction;

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        public override void PrePlaceSetup(float minX, float maxX, float maxWidth)
        {
            base.PrePlaceSetup(minX, maxX, maxWidth);
            _speed = Random.Range(_minSpeed, _maxSpeed);
            _direction = Random.Range(0, 2) * 2 - 1;
        }

        //############################################################################################
        // PROTECTED METHODS
        //############################################################################################
        protected override void FixedUpdate()
        {
            _rigitbody.MoveRotation(_rigitbody.rotation + (45.0f + _speed) * _direction * Time.fixedDeltaTime);
        }
    }
}
