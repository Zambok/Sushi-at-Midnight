using UnityEngine;
using UnityEngine.SceneManagement;

public class ServingController : MonoBehaviour
{
    [SerializeField] private OrderManager _orderManager;
    [SerializeField] private ScoreManager _scoreManager;

    private SushiPlate _heldPlate;

    public bool HasPlate => _heldPlate != null;

    public void HoldPlate(SushiPlate plate)
    {
        _heldPlate = plate;
    }

    public void ClearPlate()
    {
        _heldPlate = null;
    }

    public void ServeToCustomer(Customer customer)
    {
        if (_heldPlate == null || customer == null)
        {
            return;
        }

        float qualityScore;
        OrderResult result = _orderManager.TryCompleteOrder(customer, _heldPlate, out qualityScore);

        if (result == OrderResult.None)
        {
            return;
        }

        if (_scoreManager != null)
        {
            _scoreManager.ApplyOrderResult(
                customer,
                _heldPlate.TargetRecipe,
                _heldPlate.ActualParameters,
                result,
                qualityScore
            );
        }

        ClearPlate();
    }
}
