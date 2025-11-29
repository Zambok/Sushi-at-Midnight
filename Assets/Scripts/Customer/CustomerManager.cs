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
        if (_customerPrefab == null)
        {
            return;
        }

        // 일단은 항상 Normal 고객만 생성. 나중에 확률이나 조건으로 Event 추가
        GameObject customerObject = Instantiate(_customerPrefab, transform.position, Quaternion.identity);
        Customer customer = customerObject.GetComponent<Customer>();

        if (customer == null)
        {
            return;
        }

        TrySeatOrEnqueue(customer);
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
