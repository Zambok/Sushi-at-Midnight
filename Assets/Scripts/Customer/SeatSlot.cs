using UnityEngine;

public class SeatSlot : MonoBehaviour
{
    private Transform _seatPoint;
    private Customer _currentCustomer;

    public bool IsEmpty => _currentCustomer == null;
    public Transform SeatPoint => _seatPoint;
    public Customer CurrentCustomer => _currentCustomer;


    private void Start()
    {
        _seatPoint = GetComponent<Transform>();
    }

    public void AssignCustomer(Customer customer)
    {
        _currentCustomer = customer;

        if (_currentCustomer == null)
        {
            return;
        }

        _currentCustomer.SetSeat(this);
        _currentCustomer.MoveToSeat(_seatPoint.position);
    }

    public void ClearSeat()
    {
        _currentCustomer = null;
    }
}
