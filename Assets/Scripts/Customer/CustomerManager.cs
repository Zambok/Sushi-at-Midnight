using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private SeatSlot[] _seatSlots;
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private List<CustomerProfile> _availableProfiles;
    //[SerializeField] private DayManager _dayManager;

    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private int _maxActiveSeats = 4;

    private readonly Queue<Customer> _waitingQueue = new Queue<Customer>();
    private float _spawnTimer;
    private bool _isServiceRunning;

    public void BeginService()
    {
        _isServiceRunning = true;
        _spawnTimer = 0f;
    }

    public void EndService()
    {
        _isServiceRunning = false;
    }

    private void Update()
    {
        if (_isServiceRunning == false)
        {
            return;
        }

        UpdateSpawnTimer();
    }

    private void UpdateSpawnTimer()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer > 0f)
        {
            return;
        }

        _spawnTimer = _spawnInterval;
        SpawnCustomer();
    }

    private void SpawnCustomer()
    {
        // 1. 빈자리가 있는지 먼저 확인 (자리가 없으면 생성 자체를 안 함)
        SeatSlot emptySeat = FindEmptySeat();

        if (emptySeat == null)
        {
            return; // 자리가 없으므로 함수 종료
        }

        // 2. 프리팹이나 프로필 데이터가 제대로 있는지 안전 장치
        if (_customerPrefab == null || _availableProfiles == null || _availableProfiles.Count == 0)
        {
            Debug.LogWarning("CustomerManager: 프리팹이 없거나 할당된 프로필 데이터가 없습니다!");
            return;
        }

        // 3. 랜덤 프로필 선택 (여기서 50명 중 한 명을 뽑는 거야!)
        int randomIndex = Random.Range(0, _availableProfiles.Count);
        CustomerProfile selectedProfile = _availableProfiles[randomIndex];

        // 4. 손님 오브젝트 생성
        GameObject customerObject = Instantiate(_customerPrefab, transform.position, Quaternion.identity);
        Customer customer = customerObject.GetComponent<Customer>();

        if (customer == null)
        {
            return;
        }

        // [중요] 5. 선택된 프로필을 손님에게 주입! (이름, 이미지 등이 여기서 바뀜)
        customer.InitializeProfile(selectedProfile);

        // 6. 바로 자리에 앉히기
        emptySeat.AssignCustomer(customer);
    }

    private void TrySeatOrEnqueue(Customer customer)
    {
        SeatSlot emptySeat = FindEmptySeat();

        if (emptySeat != null)
        {
            emptySeat.AssignCustomer(customer);
            return;
        }

        _waitingQueue.Enqueue(customer);
        // 대기열 위치 조정, 줄 서는 연출은 나중에 구현
    }

    private SeatSlot FindEmptySeat()
    {
        int count = 0;

        foreach (SeatSlot seat in _seatSlots)
        {
            if (seat == null)
            {
                continue;
            }

            if (seat.IsEmpty)
            {
                return seat;
            }

            count++;
        }

        if (count >= _maxActiveSeats)
        {
            return null;
        }

        return null;
    }

    public void OnSeatFreed(SeatSlot seat)
    {
        if (_waitingQueue.Count == 0)
        {
            return;
        }

        Customer nextCustomer = _waitingQueue.Dequeue();
        seat.AssignCustomer(nextCustomer);
    }
}
