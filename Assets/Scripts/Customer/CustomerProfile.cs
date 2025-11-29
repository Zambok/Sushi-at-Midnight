using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CustomerProfile_",
    menuName = "SushiGame/Customer/CustomerProfile")]
public class CustomerProfile : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _id;
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _portrait;
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private CustomerType _customerType = CustomerType.Normal;

    [Header("Spawn Settings")]
    [SerializeField] private int _minDay = 1;
    [SerializeField] private int _maxDay = 999;
    [SerializeField] private float _spawnWeight = 1f;
    [SerializeField] private bool _uniquePerDay = false;

    [Header("Order Behaviour")]
    [SerializeField] private bool _canMakeCustomOrders = true;
    [SerializeField] private float _customOrderProbability = 0.5f;
    [SerializeField] private bool _canMakeMultipleOrders = false;

    [Tooltip("이 손님이 자주 시키는 기본/특별 주문 패턴 목록")]
    [SerializeField] private List<CustomerOrderPreset> _orderPresets = new List<CustomerOrderPreset>();

    [Header("Dialogue (Dialogue System for Unity)")]
    [Tooltip("처음 앉았을 때 인사 또는 소개 대화")]
    [SerializeField] private string _greetingConversation;

    [Tooltip("처음 주문할 때 사용하는 대화. 필요 없으면 비워둘 수 있음")]
    [SerializeField] private string _firstOrderConversation;

    [Tooltip("추가 주문할 때 사용하는 대화")]
    [SerializeField] private string _additionalOrderConversation;

    [Tooltip("여러 번 방문한 손님일 때 사용할 수 있는 대화")]
    [SerializeField] private string _repeatCustomerConversation;

    [Tooltip("먹고 나갈 때 사용하는 대화")]
    [SerializeField] private string _farewellConversation;

    public string Id => _id;
    public string DisplayName => _displayName;
    public Sprite Portrait => _portrait;
    public GameObject CustomerPrefab => _customerPrefab;
    public CustomerType CustomerType => _customerType;

    public int MinDay => _minDay;
    public int MaxDay => _maxDay;
    public float SpawnWeight => _spawnWeight;
    public bool UniquePerDay => _uniquePerDay;

    public bool CanMakeCustomOrders => _canMakeCustomOrders;
    public float CustomOrderProbability => _customOrderProbability;
    public bool CanMakeMultipleOrders => _canMakeMultipleOrders;

    public IReadOnlyList<CustomerOrderPreset> OrderPresets => _orderPresets;

    public string GreetingConversation => _greetingConversation;
    public string FirstOrderConversation => _firstOrderConversation;
    public string AdditionalOrderConversation => _additionalOrderConversation;
    public string RepeatCustomerConversation => _repeatCustomerConversation;
    public string FarewellConversation => _farewellConversation;

    public bool IsAvailableOnDay(int day)
    {
        return day >= _minDay && day <= _maxDay;
    }
}

[System.Serializable]
public class CustomerOrderPreset
{
    [Header("Order Identity")]
    [SerializeField] private string _orderId;

    [Header("Base Recipe")]
    [SerializeField] private SushiRecipe _baseRecipe;

    [Header("Optional Custom Request")]
    [SerializeField] private SushiCustomRequest _customRequest;

    [Header("Weight")]
    [Tooltip("이 손님이 여러 주문 패턴을 가질 때 상대적인 선택 비율")]
    [SerializeField] private float _weight = 1f;

    public string OrderId => _orderId;
    public SushiRecipe BaseRecipe => _baseRecipe;
    public SushiCustomRequest CustomRequest => _customRequest;
    public float Weight => _weight;
}
