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

    // [추가] 카메라 컨트롤러나 주방 컨트롤러 참조가 필요할 수 있음
    // (싱글톤으로 접근한다면 필수는 아님)

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
    // [새로 추가된 로직] 주방 <-> 홀 전환 관리
    // -----------------------------------------------------------------------

    /// <summary>
    /// 주문을 수락하고 주방으로 이동합니다. (UI 버튼에서 호출)
    /// </summary>
    /// <param name="order">만들어야 할 주문 정보</param>
    public void GoToKitchen(Order order)
    {
        // 이미 요리 중이거나 게임 중이 아니면 무시
        if (CurrentState != GameState.Playing)
        {
            Debug.LogWarning("Cannot go to kitchen: Game is not in Playing state.");
            return;
        }

        // 1. 상태 변경
        ChangeState(GameState.Crafting);

        // 2. 카메라 이동 (Cinemachine)
        if (CameraController.Instance != null)
        {
            CameraController.Instance.MoveToKitchen();
        }

        // 3. 주방 컨트롤러에게 주문 전달 (초밥 만들기 세팅)
        // (SushiCraftController를 싱글톤이나 FindObject로 찾아야 함)
        var craftController = FindFirstObjectByType<SushiCraftController>();
        if (craftController != null)
        {
            craftController.StartCraft(order.BaseRecipe); // 혹은 Order 전체 전달
        }

        // 4. (선택) UI 매니저가 있다면 주방 UI 켜기
        // UIManager.Instance.SetMode(UIMode.Kitchen);
    }

    /// <summary>
    /// 요리를 마치거나 취소하고 홀로 복귀합니다.
    /// </summary>
    public void ReturnToHall()
    {
        // 요리 중이 아니면 무시
        if (CurrentState != GameState.Crafting)
        {
            return;
        }

        // 1. 상태 변경
        ChangeState(GameState.Playing);

        // 2. 카메라 이동
        if (CameraController.Instance != null)
        {
            CameraController.Instance.MoveToCounter();
        }

        // 3. (선택) UI 매니저가 있다면 홀 UI 켜기
        // UIManager.Instance.SetMode(UIMode.Hall);
    }

    // -----------------------------------------------------------------------

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing && CurrentState != GameState.Crafting)
        {
            return;
        }

        // 이전 상태 저장 필요할 수도 있음 (Pause UI 등에서 처리)
        ChangeState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        // 돌아갈 때는 일단 Playing으로 가되, 
        // 만약 요리 중에 일시정지했다면 Crafting으로 가야 하는 로직 추가 필요
        // (간단하게 구현하려면 Pause 직전 상태를 변수에 저장해둬야 함)

        ChangeState(GameState.Playing); // 임시 복귀
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
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"Game State Changed: {newState}");
    }
}