using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SCS.Scripts.GamePlay
{
    public class Player : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Player")]
        [SerializeField] private float _playerSpeed = 6.0f;
        [SerializeField] private float _playerJumpForce = 10.0f;

        [Header("Platform")]
        [SerializeField] private LayerMask _platformLayerMask;

        [Header("Sound")]
        [SerializeField] private AudioClip _playerJumpSound;

        private Rigidbody2D _playerRigitbody;
        private Animator _playerAnimator;
        private CircleCollider2D _playerCollider;
        private AudioSource _playerAudioSource;

        private Vector3 _playerLeftTurn;
        private Vector3 _playerRightTurn;
        private Vector2 _playerMove;

        private bool _playedPush;
        private Vector2 _playerPushDirection;
        private float _playerPushForce;
        
        private float _playerDistanceToPlatform;

        private InputSystem _inputSystem;

        private float _screenMinX;
        private float _screenMaxX;

        private Rigidbody2D _platformRigitbody;

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        public void PlayerPush(Vector2 direction, float force)
        {
            _playerPushDirection = direction;
            _playerPushForce = force;
            _playedPush = true;
        }

        public void PlatformRigitbody(Rigidbody2D platformRigitbody)
        {
            _platformRigitbody = platformRigitbody;
        }
        
        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private void OnValidate()
        {
            if (_platformLayerMask.IsUnityNull())
                throw new NullReferenceException(_platformLayerMask.ToString());
        }

        private void Awake()
        {
            _playerRigitbody = GetComponent<Rigidbody2D>();
            _playerAnimator = GetComponent<Animator>();
            _playerCollider = GetComponent<CircleCollider2D>();
            _playerAudioSource = GetComponent<AudioSource>();

            _playerLeftTurn = new Vector3(-_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);
            _playerRightTurn = new Vector3(_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);

            _playedPush = false;

            _playerDistanceToPlatform = _playerCollider.radius * _playerRigitbody.transform.localScale.y + 0.05f;

            _inputSystem = new InputSystem();

            _screenMinX = SCS.Scripts.Core.GameManager.Instance.ScreenMinX();
            _screenMaxX = SCS.Scripts.Core.GameManager.Instance.ScreenMaxX();
        }

        private void FixedUpdate()
        {
            _playerAnimator.SetBool("Jump", !_platformRigitbody);

            // run
            _playerRigitbody.linearVelocityX = _playerMove.x * _playerSpeed + ((_platformRigitbody) ? _platformRigitbody.linearVelocityX : 0);

            // push
            if (_playedPush)
            {
                _playerRigitbody.AddForce(_playerPushDirection * _playerPushForce, ForceMode2D.Impulse);
                _playerAudioSource.PlayOneShot(_playerJumpSound);
                _playedPush = false;
            }

            // left <> right
            if (_playerRigitbody.position.x < _screenMinX - 0.5f || _playerRigitbody.position.x > _screenMaxX + 0.5)
            {
                _playerRigitbody.position = new Vector2(_playerRigitbody.position.x * -0.99f, _playerRigitbody.position.y);
            }
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
            _inputSystem.Player.Move.performed += OnMove;
            _inputSystem.Player.Move.canceled += OnMove;
            _inputSystem.Player.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            _inputSystem.Player.Move.performed -= OnMove;
            _inputSystem.Player.Move.canceled -= OnMove;
            _inputSystem.Player.Jump.performed -= OnJump;
            _inputSystem.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            // get current move
            _playerMove = context.ReadValue<Vector2>();

            // reset move. controller noise
            if (_playerMove.x > -0.1 && _playerMove.x < 0.1)
                _playerMove.x = 0;

            // turn player
            if (_playerMove.x < 0)
                _playerRigitbody.transform.localScale = _playerLeftTurn;
            if (_playerMove.x > 0)
                _playerRigitbody.transform.localScale = _playerRightTurn;

            // enable animation run
            _playerAnimator.SetBool("Run", _playerMove.x != 0);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (_platformRigitbody && !_playedPush)
            {
                _playerPushDirection = Vector2.up;
                _playerPushForce = _playerJumpForce;
                _playedPush = true;
            }
        }
    }
}
