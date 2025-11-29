using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.None;

    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private OrderManager _orderManager;
    [SerializeField] private ScoreManager _scoreManager;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        ChangeState(GameState.Ready);

        StartGame();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 필요하면 DontDestroyOnLoad(gameObject); 추가
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);

        if (_customerManager != null)
        {
            _customerManager.BeginService();
        }

        if (_scoreManager != null)
        {
            _scoreManager.ResetScore();
        }
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        ChangeState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        ChangeState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        ChangeState(GameState.Result);

        if (_customerManager != null)
        {
            _customerManager.EndService();
        }
    }

    private void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        // 나중에 상태 변경 이벤트를 추가할 수 있음
    }
}
