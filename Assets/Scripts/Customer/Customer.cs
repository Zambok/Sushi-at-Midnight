using UnityEngine;
using DG.Tweening;
using PixelCrushers.DialogueSystem;

public class Customer : MonoBehaviour
{
    [SerializeField] private CustomerProfile _profile;
    [SerializeField] private CustomerType _customerType = CustomerType.Normal;
    [SerializeField] private float _basePatienceTime = 10f;

    [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;     // 입장 페이드 시간
    [SerializeField] private float _bounceDuration = 0.3f;   // 튀어오르는 시간
    [SerializeField] private Vector3 _bounceStrength = new Vector3(0.1f, 0.1f, 0); // 튀어오르는 강도
    [SerializeField] private int _bounceVibrato = 10;        // 떨림 정도 (기본 10)
    [SerializeField] private float _bounceElasticity = 1f;   // 탄성 (0~1)

  
    private CustomerState _currentState = CustomerState.None;
    private float _currentPatienceTime;
    private SeatSlot _currentSeat;
    private SpriteRenderer _spriteRenderer;
    private DialogueActor _dialogueActor;

    public CustomerProfile Profile => _profile;

    // Order 관련 참조는 Order 폴더 설계 후 연결 예정
    private Order _currentOrder;

    public CustomerType CustomerType => _customerType;
    public CustomerState CurrentState => _currentState;
    public SeatSlot CurrentSeat => _currentSeat;
    public Order CurrentOrder => _currentOrder;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _dialogueActor = GetComponent<DialogueActor>();
        if (_dialogueActor == null)
        {
            _dialogueActor = gameObject.AddComponent<DialogueActor>();
        }
    }

    private void OnEnable()
    {
        InitializeCustomer();
    }

    private void Update()
    {
        UpdatePatience();
    }

    private void InitializeCustomer()
    {
        _currentPatienceTime = _basePatienceTime;
        ChangeState(CustomerState.Entering);
    }

    public void InitializeProfile(CustomerProfile profile)
    {
        _profile = profile;

        // 외형 초기화
        gameObject.name = $"Customer_{_profile.DisplayName}";
        if (_spriteRenderer != null && _profile.GetSprite(CustomerEmotion.Default) != null)
        {
            _spriteRenderer.sprite = _profile.GetSprite(CustomerEmotion.Default);
        }

        // 대화창에 표시될 이름을 프로필의 DisplayName으로 변경
        _dialogueActor.actor = _profile.DisplayName;

        // 기본 표정으로 시작
        SetEmotion(CustomerEmotion.Default);
    }

    public void StartConversation(string conversationTitle)
    {
        // Dialogue Manager에게 대화 시작 요청
        // 첫 번째 인자: 대화 제목 (Database에 있는 Conversation Title)
        // 두 번째 인자: 대화 주체 (Player)
        // 세 번째 인자: 대화 상대 (이 손님)
        DialogueManager.StartConversation(conversationTitle, null, this.transform);
    }

    public void SetEmotion(CustomerEmotion emotion)
    {
        if (_profile == null || _spriteRenderer == null) return;

        Sprite targetSprite = _profile.GetSprite(emotion);

        if (targetSprite != null)
        {
            _spriteRenderer.sprite = targetSprite;

            // 이미지 바뀌면서 통통 튀기!
            PlayBounceAnimation();
        }
    }

    private void PlayBounceAnimation()
    {
        // 혹시 실행 중인 트윈이 있다면 중지하고 리셋 (겹침 방지)
        transform.DOKill();

        // PunchScale: '통' 하고 커졌다가 돌아오는 효과
        transform.DOPunchScale(_bounceStrength, _bounceDuration, _bounceVibrato, _bounceElasticity);
    }

    private void UpdatePatience()
    {
        if (_currentState != CustomerState.WaitingOrder &&
            _currentState != CustomerState.WaitingFood)
        {
            return;
        }

        _currentPatienceTime -= Time.deltaTime;

        if (_currentPatienceTime <= 0f)
        {
            HandlePatienceTimeout();
        }
    }

    private void HandlePatienceTimeout()
    {
        // 나중에 실패 처리, 손님 화남, 점수 패널티 등 연결
        LeaveRestaurant();
    }

    private void ChangeState(CustomerState newState)
    {
        if (_currentState == newState)
        {
            return;
        }

        _currentState = newState;
    }

    public void SetSeat(SeatSlot seat)
    {
        _currentSeat = seat;
    }

    public void MoveToSeat(Vector3 seatPosition)
    {
        transform.position = seatPosition + new Vector3(0, 0.339f, -0.1f);

        // Dotween: 일단 검은색으로 시작
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.black;

            // 원래 색(White)으로 페이드 인
            _spriteRenderer.DOColor(Color.white, _fadeDuration)
                .SetEase(Ease.OutQuad); // 부드러운 감속 효과
        }

        ChangeState(CustomerState.Ordering);

        // 자리에 앉으면 인사 대화 시작!
        if (!string.IsNullOrEmpty(_profile.GreetingConversation))
        {
            StartConversation(_profile.GreetingConversation);
        }
    }

    public void StartWaitingFood()
    {
        _currentPatienceTime = _basePatienceTime;
        ChangeState(CustomerState.WaitingFood);
    }

    public void ReceiveFood(SushiPlate sushiPlate)
    {
        // 여기서 Order와 SushiPlate를 비교하는 로직은 Order 폴더 설계 후 추가
        ChangeState(CustomerState.Eating);
    }

    public void FinishEating()
    {
        LeaveRestaurant();
    }

    public void LeaveRestaurant()
    {
        ChangeState(CustomerState.Leaving);

        if (_currentSeat != null)
        {
            _currentSeat.ClearSeat();
            _currentSeat = null;
        }

        Destroy(gameObject);
    }

    public void SetOrder(Order order)
    {
        _currentOrder = order;
        ChangeState(CustomerState.WaitingFood);
    }
}
