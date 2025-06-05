using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace SCS.Scripts.GamePlay.Platforms
{
    public class MovePlatform : BasePlatform
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Speed")]
        [SerializeField] private float _minSpeed = 2;
        [SerializeField] private float _maxSpeed = 5;

        private float _distance = 0;
        private float _speed = 0;
        private float _minX;
        private float _maxX;
        private int _direction = 0;

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        public override void PrePlaceSetup(float minX, float maxX, float maxWidth)
        {
            base.PrePlaceSetup(minX, maxX, maxWidth);
            _distance = (maxWidth > _realWidth) ? maxWidth - _realWidth : 0;
            _minX = minX + _spriteRenderer.bounds.size.x / 2;
            _maxX = maxX - _spriteRenderer.bounds.size.x / 2;
            _speed = Random.Range(_minSpeed, _maxSpeed);
            _direction = Random.Range(0, 2) * 2 - 1;
            _width = _realWidth + _distance;
        }

        public override bool CanTakeFullLine()
        {
            return true;
        }

        public override Vector2 GetPosition()
        {
            return new Vector2(_minX + _distance / 2, transform.position.y);
        }

        //############################################################################################
        // PROTECTED METHODS
        //############################################################################################
        protected override void FixedUpdate()
        {
            if (transform.position.x < _minX )
                _direction = 1;
            if (transform.position.x > _maxX)
                _direction = -1;
            _rigitbody.linearVelocityX = _speed * _direction;
        }
    }
}
