using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private UnityEvent onGameOver;
    [SerializeField] private UnityEvent onBeginGame;

    [SerializeField] private GameObject UIElements;
    [SerializeField] private GameObject UIOverlay;
    [SerializeField] private GameObject endOverlay;

    private bool gameStarted;

    private void Awake()
    {
        gameStarted = false;
        endOverlay.SetActive(false);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        InputSystem.onAnyButtonPress.CallOnce(BeginGame);
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame) SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        Invoke("InvokeGameOver", 2.5f);   
    }

    public void InvokeGameOver()
    {
        onGameOver?.Invoke();
        endOverlay.SetActive(true);
    }

    private void BeginGame(InputControl control)
    {
        if (!gameStarted)
        {
            gameStarted = true;
            onBeginGame?.Invoke();
            UIElements.SetActive(true);
            UIOverlay.SetActive(false);
        }
    }
}