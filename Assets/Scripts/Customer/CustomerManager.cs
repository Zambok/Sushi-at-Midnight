using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [추가] 2인 손님 조합을 정의하기 위한 클래스
[System.Serializable]
public class DuoScenario
{
    public string ScenarioName; // 구분용 이름 (예: 경찰_학생_이벤트)
    public CustomerProfile LeftProfile;
    public CustomerProfile RightProfile;
    [Range(0f, 1f)] public float Probability = 0.1f; // 등장 확률
}

public class CustomerManager : MonoBehaviour
{
    [Header("Seat Configurations")]
    [Tooltip("혼자 온 일반 손님이 앉을 자리")]
    [SerializeField] private SeatSlot _seatCenter;

    [Tooltip("2인 손님(왼쪽)")]
    [SerializeField] private SeatSlot _seatLeft;

    [Tooltip("2인 손님(오른쪽)")]
    [SerializeField] private SeatSlot _seatRight;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject _customerPrefab;

    [Tooltip("일반 손님(1인)으로 등장할 프로필 목록")]
    [SerializeField] private List<CustomerProfile> _normalProfiles;

    [Tooltip("이벤트성 2인 손님 조합 목록")]
    [SerializeField] private List<DuoScenario> _eventDuoPresets;

    [Tooltip("손님이 나가고 다음 손님이 올 때까지 대기 시간")]
    [SerializeField] private float _nextSpawnDelay = 2.0f;

    [Tooltip("이벤트(2인)가 발생할 확률 (0~1)")]
    [SerializeField] private float _eventChance = 0.2f;

    private bool _isServiceRunning;
    private int _currentActiveCustomers = 0; // 현재 앉아있는 손님 수

    // --- 초기화 및 실행 ---
    public void BeginService()
    {
        _isServiceRunning = true;
        // 서비스 시작 시 첫 손님 호출
        StartCoroutine(SpawnRoutine(_nextSpawnDelay));
    }

    public void EndService()
    {
        _isServiceRunning = false;
        StopAllCoroutines();
    }

    // --- 손님 퇴장 감지 ---
    // Customer 스크립트에서 퇴장할 때 이 함수를 호출해줌
    public void OnCustomerExit(Customer customer)
    {
        _currentActiveCustomers--;

        // 앉아있는 손님이 모두 떠났다면 다음 손님 준비
        if (_currentActiveCustomers <= 0)
        {
            _currentActiveCustomers = 0; // 안전장치

            if (_isServiceRunning)
            {
                StartCoroutine(SpawnRoutine(_nextSpawnDelay));
            }
        }
    }

    private IEnumerator SpawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_isServiceRunning)
        {
            SpawnNextGroup();
        }
    }

    private void SpawnNextGroup()
    {
        // 1. 이벤트(2인) 발생 여부 체크
        // (나중에는 StoryManager 등에서 강제로 이벤트를 발생시킬 수도 있음)
        bool isEvent = Random.value < _eventChance;

        if (isEvent && _eventDuoPresets.Count > 0)
        {
            if (_eventDuoPresets == null) return;
            SpawnDuoEvent();
        }
        else
        {
            SpawnSoloNormal();
        }
    }

    // --- 1인 일반 손님 생성 ---
    private void SpawnSoloNormal()
    {
        if (_normalProfiles == null || _normalProfiles.Count == 0) return;

        // 랜덤 프로필 선택
        CustomerProfile profile = _normalProfiles[Random.Range(0, _normalProfiles.Count)];

        _currentActiveCustomers = 1;
        CreateCustomerAtSeat(_seatCenter, profile);
    }

    // --- 2인 이벤트 손님 생성 ---
    private void SpawnDuoEvent()
    {
        // 랜덤으로 시나리오 하나 선택 (가중치 로직 등 추가 가능)
        int index = Random.Range(0, _eventDuoPresets.Count);
        DuoScenario scenario = _eventDuoPresets[index];

        _currentActiveCustomers = 2;
        CreateCustomerAtSeat(_seatLeft, scenario.LeftProfile);
        CreateCustomerAtSeat(_seatRight, scenario.RightProfile);

        // TODO: 여기서 두 손님에게 "너네는 일행이야"라고 알려주는 로직 추가 가능
        // 예: customerL.SetPartner(customerR);
    }

    private void CreateCustomerAtSeat(SeatSlot seat, CustomerProfile profile)
    {
        if (_customerPrefab == null) return;

        // 생성
        GameObject obj = Instantiate(_customerPrefab, transform.position, Quaternion.identity);
        Customer customer = obj.GetComponent<Customer>();

        if (customer != null)
        {
            customer.InitializeProfile(profile);

            // [중요] 나갈 때 나한테 보고해! (콜백 연결)
            customer.SetExitCallback(OnCustomerExit);

            seat.AssignCustomer(customer);
        }
    }
}