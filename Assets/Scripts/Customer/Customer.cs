using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private CustomerProfile _profile;
    [SerializeField] private CustomerType _customerType = CustomerType.Normal;
    [SerializeField] private float _basePatienceTime = 10f;

    private CustomerState _currentState = CustomerState.None;
    private float _currentPatienceTime;
    private SeatSlot _currentSeat;

    public CustomerProfile Profile => _profile;

    // Order 관련 참조는 Order 폴더 설계 후 연결 예정
    private Order _currentOrder;

    public CustomerType CustomerType => _customerType;
    public CustomerState CurrentState => _currentState;
    public SeatSlot CurrentSeat => _currentSeat;
    public Order CurrentOrder => _currentOrder;

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
        // 일단 위치만 순간 이동. 나중에 이동 애니메이션이나 Path 추가 가능
        transform.position = seatPosition;
        ChangeState(CustomerState.Ordering);
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
