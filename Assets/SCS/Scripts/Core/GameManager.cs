using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCS.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        //############################################################################################
        // PROPERTIES
        //############################################################################################
        public static GameManager Instance { get; private set; }

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        public float ScreenMinX()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        }

        public float ScreenMaxX()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        }

        public float ScreenWidth()
        {
            return ScreenMaxX() - ScreenMinX();
        }

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
