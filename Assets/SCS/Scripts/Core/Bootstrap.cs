using SCS.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    //############################################################################################
    // FIELDS
    //############################################################################################
    [SerializeField] private string _startScene;

    //############################################################################################
    // PRIVATE METHODS
    //############################################################################################
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_startScene))
            throw new UnassignedReferenceException("_startScene");
    }

    private void Awake()
    {
        // create GameManager
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        SceneManager.LoadScene(_startScene);
    }
}
