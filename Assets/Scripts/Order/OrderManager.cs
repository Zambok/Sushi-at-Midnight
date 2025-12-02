using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private SushiRecipe[] _availableRecipes;
    [SerializeField] private float _defaultOrderTimeLimit = 15f;
    [SerializeField] private float _customOrderProbability = 0.4f;
    private readonly List<Order> _activeOrders = new List<Order>();

    public IReadOnlyList<Order> ActiveOrders => _activeOrders;

    private void Update()
    {
        TickOrders();
    }

    private void TickOrders()
    {
        float deltaTime = Time.deltaTime;

        for (int i = _activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = _activeOrders[i];
            order.Tick(deltaTime);

            if (order.State == OrderState.Failed)
            {
                HandleOrderFailed(order);
                _activeOrders.RemoveAt(i);
            }
        }
    }

    // OrderManager.cs 안에 추가

    // 특정 레시피 이름으로 주문을 강제 생성하는 함수 (대화 연동용)
    public Order CreateSpecificOrder(Customer customer, string recipeName, string requestType = "")
    {
        // 1. 이름으로 레시피 찾기
        SushiRecipe targetRecipe = null;
        foreach (var recipe in _availableRecipes)
        {
            if (recipe.name.Equals(recipeName, System.StringComparison.OrdinalIgnoreCase) ||
                recipe.SushiType.ToString().Equals(recipeName, System.StringComparison.OrdinalIgnoreCase))
            {
                targetRecipe = recipe;
                break;
            }
        }

        if (targetRecipe == null)
        {
            Debug.LogError($"OrderManager: '{recipeName}' 레시피를 찾을 수 없습니다!");
            return null;
        }

        // 2. 요청사항(Request) 처리 (필요시 확장 가능)
        SushiCustomRequest customRequest = null;
        if (!string.IsNullOrEmpty(requestType))
        {
            if (requestType == "RiceLess") customRequest = SushiCustomRequest.CreateRiceLess();
            else if (requestType == "ThickFish") customRequest = SushiCustomRequest.CreateThickFish();
            // ... 필요한 만큼 추가
        }

        // 3. 주문 생성 및 등록
        Order order = new Order(targetRecipe, customRequest, customer, _defaultOrderTimeLimit);
        _activeOrders.Add(order);
        customer.SetOrder(order);

        return order;
    }

    public Order CreateOrderForCustomer(Customer customer)
    {
        SushiRecipe recipe = GetRandomRecipe();

        if (recipe == null)
        {
            return null;
        }

        SushiCustomRequest customeRequest = TryCreateCustomRequestForCustomer(customer, recipe);

        Order order = new Order(recipe, customeRequest, customer, _defaultOrderTimeLimit);
        _activeOrders.Add(order);

        customer.SetOrder(order);

        // 나중에 UI에 주문 표시 이벤트 호출 가능
        return order;
    }

    private SushiCustomRequest TryCreateCustomRequestForCustomer(Customer customer, SushiRecipe recipe)
    {
        float roll = Random.value;

        if (roll > _customOrderProbability)
        {
            return null;
        }

        float subRoll = Random.value;

        if (subRoll < 0.25f)
        {
            return SushiCustomRequest.CreateRiceLess(-0.3f);
        }

        if (subRoll < 0.5f)
        {
            return SushiCustomRequest.CreateThickFish(0.3f);
        }

        if (subRoll < 0.75f)
        {
            return SushiCustomRequest.CreateMoreWasabi(0.4f);
        }

        return SushiCustomRequest.CreateSoftPress(-0.3f);
    }

    public OrderResult TryCompleteOrder(Customer customer, SushiPlate sushiPlate, out float qualityScore)
    {
        qualityScore = 0f;

        Order order = FindOrderByCustomer(customer);

        if (order == null)
        {
            return OrderResult.None;
        }

        OrderResult result = order.Complete(sushiPlate, out qualityScore);
        _activeOrders.Remove(order);

        HandleOrderCompleted(order, result, qualityScore);

        return result;
    }


    private Order FindOrderByCustomer(Customer customer)
    {
        for (int i = 0; i < _activeOrders.Count; i++)
        {
            if (_activeOrders[i].Owner == customer)
            {
                return _activeOrders[i];
            }
        }

        return null;
    }

    private SushiRecipe GetRandomRecipe()
    {
        if (_availableRecipes == null || _availableRecipes.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, _availableRecipes.Length);
        return _availableRecipes[index];
    }
    private void HandleOrderCompleted(Order order, OrderResult result, float qualityScore)
    {
        // 여기서는 점수 직접 계산하지 않고,
        // ScoreManager가 처리하도록 넘기는 역할만 해도 됨.
    }

    private void HandleOrderFailed(Order order)
    {
        // 실패 시 패널티 적용, 손님 떠나게 만들기 등 연결
        Customer owner = order.Owner;

        if (owner != null)
        {
            owner.LeaveRestaurant();
        }
    }
}
