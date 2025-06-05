using SCS.Scripts.GamePlay.Platforms;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace SCS.Scripts.GamePlay
{
    public struct PlatformPlace
    {
        public float minX;
        public float maxX;
        public float width;
        public PlatformPlace(float minX, float maxX, float width)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.width = width;
        }
    }
    public class PlatformSpawner : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Platform Prefabs List")]
        [SerializeField] private List<SCS.Scripts.GamePlay.Platforms.BasePlatform> _platformPrefabs;

        [Header("BottomTarget Tramsform")]
        [SerializeField] private Transform _bottomTarget;

        [Header("PlayerTarget Transform")]
        [SerializeField] private Transform _playerTarget;

        [Header("Spawn Settings")]
        [SerializeField] private int _maxStage = 20;
        [SerializeField] private int _maxSpawnByDirection = 5;
        [SerializeField] private float _platformYFromPrevious = 4.0f;
        [SerializeField] private float _platformMinXFromPrevious = -4.0f;
        [SerializeField] private float _platformMaxXFromPrevious = 4.0f;
        [SerializeField] private int _minPlatformsByStage = 1;
        [SerializeField] private int _maxPlatformsByStage = 3;

        private float _screenMinX;
        private float _screenMaxX;

        private List<BasePlatform> _platformPool;
        private List<List<BasePlatform>> _platformSpawnList;

        private int _playerStage = 0;
        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private void OnValidate()
        {
            if (_platformPrefabs == null || _platformPrefabs.Count == 0)
                throw new UnassignedReferenceException(nameof(_platformPrefabs));
            if (_bottomTarget.IsUnityNull())
                throw new UnassignedReferenceException(nameof(_bottomTarget));
            if (_playerTarget.IsUnityNull())
                throw new UnassignedReferenceException(nameof(_playerTarget));
        }

        private void Awake()
        {
            _screenMinX = SCS.Scripts.Core.GameManager.Instance.ScreenMinX();
            _screenMaxX = SCS.Scripts.Core.GameManager.Instance.ScreenMaxX();

            _platformPool = new List<BasePlatform>();
            _platformSpawnList = new List<List<BasePlatform>>();

            SpawnPlatform(_maxSpawnByDirection, 1);
        }

        private void FixedUpdate()
        {
            CheckPlayerPlace();
        }

        private void CheckPlayerPlace()
        {
            int playerStage = 0;
            for (int i = _platformSpawnList.Count - 1; i >= 0; i--)
                if (_playerTarget.position.y > _platformSpawnList[i].First().transform.position.y)
                {
                    playerStage = _platformSpawnList[i].First().Stage;
                    break;
                }
            if (_playerStage != playerStage)
            {
                Debug.Log($"Player on stage {playerStage}");
                _playerStage = playerStage;
                // up
                int countPlatforms = _platformSpawnList.Last().First().Stage - _playerStage;
                if (countPlatforms > _maxSpawnByDirection)
                    DropPlatform(countPlatforms - _maxSpawnByDirection, 1);
                else if (countPlatforms < _maxSpawnByDirection)
                    SpawnPlatform(_maxSpawnByDirection - countPlatforms, 1);

                // bottom
                countPlatforms = _playerStage - _platformSpawnList.First().First().Stage;
                if (countPlatforms > _maxSpawnByDirection)
                    DropPlatform(countPlatforms - _maxSpawnByDirection, -1);
                else if (countPlatforms < _maxSpawnByDirection)
                    SpawnPlatform(_maxSpawnByDirection - countPlatforms, -1);
            }
        }

        private void SpawnPlatform(int count, int direction)
        {
            for (int i = 0; i < count; i++)
            {
                int nextStage = (_platformSpawnList.Count() > 0) ? (((direction == 1) ? _platformSpawnList.Last().First().Stage : _platformSpawnList.First().First().Stage) + direction) : 1;
                if (nextStage < 1 || nextStage > _maxStage)
                    return;
                int maxPlatformsByStage = Random.Range(_minPlatformsByStage, _maxPlatformsByStage + 1);
                List<BasePlatform> platformGroup = new List<BasePlatform>();
                while(true)
                {
                    // too many platforms
                    if (platformGroup.Count() == maxPlatformsByStage)
                        break;
                    // prepare new platform
                    BasePlatform platformType = _platformPrefabs[Random.Range(0, _platformPrefabs.Count())];
                    BasePlatform platform = GetFreePlatformFromPool(ref platformType);
                    PlatformPlace maxFreePlace = GetMaxFreePlace(ref platformGroup, platform.CanTakeFullLine());
                    platform.Stage = nextStage;
                    platform.PrePlaceSetup(maxFreePlace.minX, maxFreePlace.maxX, maxFreePlace.width);
                    // calculate new place
                    if (maxFreePlace.width < platform.GetWidth())
                        break;
                    float platformX = Random.Range(maxFreePlace.minX + ((maxFreePlace.minX == _screenMinX) ? 0 : platform.GetWidth() / 2),
                        maxFreePlace.maxX - ((maxFreePlace.maxX == _screenMaxX) ? 0 : platform.GetWidth() / 2));
                    float platformY;
                    if (_platformSpawnList.Count() == 0)
                    {
                        platformY = _bottomTarget.position.y + _platformYFromPrevious * direction;
                    }
                    else
                    {
                        BasePlatform lastPlatform = (direction == 1) ? _platformSpawnList.Last().First() : _platformSpawnList.First().First();
                        platformY = lastPlatform.transform.position.y + _platformYFromPrevious * direction;
                    }
                    // post setup platform
                    platform.transform.position = new Vector3(platformX, platformY, 0);
                    platform.PostPlaceSetup();
                    platform.Busy = true;
                    platformGroup.Add(platform);
                }
                if (direction == 1)
                    _platformSpawnList.Add(platformGroup);
                else
                    _platformSpawnList.Insert(0, platformGroup);
            }
        }

        private void DropPlatform(int count, int direction)
        {
            List<BasePlatform> platformGroup;
            for (int i = 0; i < count; i++)
            {
                if (_platformSpawnList.Count() == 0)
                    return;
                platformGroup = (direction == 1) ? _platformSpawnList.Last() : _platformSpawnList.First();
                foreach (BasePlatform p in platformGroup)
                    p.Busy = false;
                _platformSpawnList.RemoveAt((direction == 1) ? _platformSpawnList.Count() - 1 : 0);
            }
        }

        private BasePlatform GetFreePlatformFromPool(ref BasePlatform platformType)
        {
            // try to find free platform with correct type
            foreach (BasePlatform p in _platformPool)
                if (p.GetType() == platformType.GetType() && !p.Busy)
                    return p;

            // create new platform to pool with correct type
            BasePlatform platform = Instantiate(platformType, Vector3.zero, Quaternion.identity);
            platform.Busy = false;
            _platformPool.Add(platform);
            return platform;
        }

        private PlatformPlace GetMaxFreePlace(ref List<BasePlatform> platformGroup, bool canTakeFullLine)
        {
            if (platformGroup.Count() == 0)
                return new PlatformPlace((canTakeFullLine) ? _screenMinX : _platformMinXFromPrevious, (canTakeFullLine) ? _screenMaxX : _platformMaxXFromPrevious, _screenMaxX - _screenMinX);
            // create list with points
            List<Vector2> testList = new List<Vector2>();
            testList.Add(new Vector2(_screenMinX, 0));
            foreach (BasePlatform p in platformGroup)
                testList.Add(new Vector2(p.GetPosition().x, p.GetWidth() / 2));
            testList.Add(new Vector2(_screenMaxX, 0));
            // sort list
            testList.Sort((a, b) => a.x.CompareTo(b.x));
            // find the biggest place
            PlatformPlace result = new PlatformPlace(0, 0, 0);
            for (int i = 0; i < testList.Count() - 1; i++)
            {
                float minX = testList[i].x + testList[i].y;
                float maxX = testList[i + 1].x - testList[i + 1].y;
                float width = maxX - minX;
                if (width > result.width)
                {
                    result.minX = minX;
                    result.maxX = maxX;
                    result.width = width;
                }
            }
            return result;
        }
    }
}