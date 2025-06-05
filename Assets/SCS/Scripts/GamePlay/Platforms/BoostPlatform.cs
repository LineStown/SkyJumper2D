using UnityEngine;

namespace SCS.Scripts.GamePlay.Platforms
{
    public class BoostPlatform : BasePlatform
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Boost Power")]
        [SerializeField] private float _minPower = 10;
        [SerializeField] private float _maxPower = 20;

        //############################################################################################
        // PROTECTED METHODS
        //############################################################################################
        protected override void OnPlayerLanding(Player player)
        {
            base.OnPlayerLanding(player);
            Vector2 direction = new Vector2(0, 1);
            player.PlayerPush(direction, Random.Range(_minPower, _maxPower));
        }
    }
}
