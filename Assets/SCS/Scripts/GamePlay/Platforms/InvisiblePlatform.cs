using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace SCS.Scripts.GamePlay.Platforms
{
    public class InvisiblePlatform : BasePlatform
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Delay Timer")]
        [SerializeField] private float _delayBeforeShake = 0.5f;
        [SerializeField] private float _shakeDuration = 0.5f;

        [Header("Shake")]
        [SerializeField] private float _shakeStrength = 0.05f;
        [SerializeField] private float _shakeFrequency = 20.0f;

        [Header("Respawn Timer")]
        [SerializeField] private float _respawnDelay = 3.0f;

        [Header("Sound")]
        [SerializeField] private AudioClip _invisibleSound;

        private Player _player;
        private bool _started;

        //############################################################################################
        // PROTECTED METHODS
        //############################################################################################
        protected override void OnPlayerLanding(Player player)
        {
            base.OnPlayerLanding(player);
            if (!_started)
            {
                _started = true;
                _player = player;
                StartCoroutine(DisappearCycle());
            }
        }

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private IEnumerator DisappearCycle()
        {
            yield return new WaitForSeconds(_delayBeforeShake);
            yield return StartCoroutine(ShakeScale());
            yield return new WaitForSeconds(_respawnDelay);
            RestorePlatform();
        }

        private IEnumerator ShakeScale()
        {
            float time = 0f;
            while (time < _shakeDuration)
            {
                float noise = Mathf.Sin(time * _shakeFrequency * Mathf.PI * 2f);
                float scaleOffset = 1 + noise * _shakeStrength;
                transform.localScale = new Vector3(scaleOffset, scaleOffset, 1f);
                time += Time.deltaTime;
                yield return null;
            }
            transform.localScale = Vector3.one;
            GetComponent<AudioSource>().PlayOneShot(_invisibleSound);
            if (_player.gameObject.transform.parent == this)
                _player.gameObject.transform.SetParent(null);
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
        }
        
        private void RestorePlatform()
        {
            if (_started)
            {
                _started = false;
                _collider.enabled = true;
                _spriteRenderer.enabled = true;
            }
        }
    }
}
