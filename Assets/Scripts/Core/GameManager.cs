using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.None;

    [Header("Managers")]
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

    // -----------------------------------------------------------------------
    // [수정된 로직] 주방 <-> 홀 전환 관리 (통합 컨트롤러 사용)
    // -----------------------------------------------------------------------

    /// <summary>
    /// 주문을 수락하고 주방으로 이동합니다.
    /// </summary>
    public void GoToKitchen(Order order)
    {
        if (CurrentState != GameState.Playing)
        {
            Debug.LogWarning("Cannot go to kitchen: Game is not in Playing state.");
            return;
        }

        // 1. 상태 변경
        ChangeState(GameState.Crafting);

        // 2. 통합 뷰 컨트롤러를 통해 주방으로 이동 (Fade 효과 포함)
        if (KitchenStationController.Instance != null)
        {
            KitchenStationController.Instance.EnterKitchenMode();
        }
        else
        {
            Debug.LogError("KitchenStationController가 씬에 없습니다!");
        }

        // 3. 주방 컨트롤러에게 주문 전달 (초밥 만들기 세팅)
        // (호환성을 위해 FindObjectOfType 사용 권장)
        var craftController = FindObjectOfType<SushiCraftController>();
        if (craftController != null)
        {
            craftController.StartCraft(order.BaseRecipe);
        }
    }

    /// <summary>
    /// 요리를 마치거나 취소하고 홀로 복귀합니다.
    /// </summary>
    public void ReturnToHall()
    {
        if (CurrentState != GameState.Crafting)
        {
            return;
        }

        // 1. 상태 변경
        ChangeState(GameState.Playing);

        // 2. 통합 뷰 컨트롤러를 통해 홀로 복귀 (Fade 효과 포함)
        if (KitchenStationController.Instance != null)
        {
            KitchenStationController.Instance.ExitKitchenMode();
        }
    }

    // -----------------------------------------------------------------------

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing && CurrentState != GameState.Crafting) return;
        ChangeState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        ChangeState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        ChangeState(GameState.Result);
        if (_customerManager != null) _customerManager.EndService();
    }

    private void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        Debug.Log($"Game State Changed: {newState}");
    }
}