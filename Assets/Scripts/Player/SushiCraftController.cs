using UnityEngine;
using UnityEngine.Events; // 이벤트 사용을 위해 추가

public enum CraftingState
{
    None,
    Rice,       // 1단계: 밥 쥐기
    Wasabi,     // 2단계: 와사비 바르기
    Fish,       // 3단계: 생선 올리기 (완성)
    Completed
}

public class SushiCraftController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _riceGrowthSpeed = 0.5f; // 밥이 커지는 속도
    [SerializeField] private Transform _riceVisual; // 밥알 오브젝트 (크기 변화용)

    [Header("Data")]
    [SerializeField] private SushiRecipe _currentRecipe;

    private SushiProcessParameters _currentParameters;
    private CraftingState _state = CraftingState.None;
    private bool _isHolding = false;

    // 외부에서 상태를 알 수 있게 프로퍼티 제공
    public CraftingState State => _state;
    public SushiRecipe CurrentRecipe => _currentRecipe;

    private void Awake()
    {
        _currentParameters = new SushiProcessParameters();
    }

    private void Update()
    {
        // 밥 쥐기 단계일 때만 입력 처리
        if (_state == CraftingState.Rice)
        {
            HandleRiceInput();
        }
    }

    // 1. 요리 시작 (GameManager가 호출)
    public void StartCraft(SushiRecipe recipe)
    {
        if (recipe == null) return;

        _currentRecipe = recipe;
        _currentParameters = new SushiProcessParameters();

        // 초기화: 밥 양 0부터 시작
        _currentParameters.RiceAmount = 0f;

        // 밥 오브젝트 초기화 (안 보이게 하거나 아주 작게)
        if (_riceVisual != null)
        {
            _riceVisual.localScale = Vector3.zero;
            _riceVisual.gameObject.SetActive(true);
        }

        // 상태 변경 -> 밥 쥐기 시작
        ChangeState(CraftingState.Rice);
        Debug.Log($" [{recipe.name}] 요리 시작! 마우스를 눌러 밥을 쥐세요.");
    }

    // 2. 밥 쥐기 입력 처리 (꾹 누르기)
    private void HandleRiceInput()
    {
        // 마우스 클릭 시작
        if (Input.GetMouseButtonDown(0))
        {
            _isHolding = true;
        }

        // 누르고 있는 동안 -> 밥 양 증가
        if (Input.GetMouseButton(0) && _isHolding)
        {
            // 0 ~ 1.2 사이로 증가 (1.0이 목표지만 실수를 위해 조금 더 허용)
            _currentParameters.RiceAmount += Time.deltaTime * _riceGrowthSpeed;

            // 시각적 피드백: 밥알 크기 키우기
            UpdateRiceVisual(_currentParameters.RiceAmount);
        }

        // 마우스 뗌 -> 확정
        if (Input.GetMouseButtonUp(0) && _isHolding)
        {
            _isHolding = false;
            Debug.Log($" 밥 쥐기 완료! (양: {_currentParameters.RiceAmount:F2})");

            // 다음 단계(와사비)로 넘어감
            ChangeState(CraftingState.Wasabi);
        }
    }

    private void UpdateRiceVisual(float amount)
    {
        if (_riceVisual == null) return;

        // 밥 양(0~1)에 따라 스케일을 0~1.5배로 조절
        float scale = Mathf.Clamp(amount, 0f, 1.5f);
        _riceVisual.localScale = Vector3.one * scale;
    }

    private void ChangeState(CraftingState newState)
    {
        _state = newState;
        // 나중에 여기에 UI 변경이나 가이드 텍스트 출력 로직 추가 가능
    }

    // ... (나머지 와사비, 생선 로직은 차차 추가) ...
}