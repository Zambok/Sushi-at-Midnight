using System.Collections.Generic;
using UnityEngine;
public enum CustomerEmotion
{
    Default,    // 기본 상태 (입장, 대기)
    Talking,    // 대화 중
    Happy,      // 맛있음 (Delicious)
    Angry,      // 맛없음 (Bad)
    Perfect,    // 완벽함 (Perfect)
    Signature   // 시그니처/스토리 이벤트
}

[CreateAssetMenu(fileName = "CustomerProfile_", menuName = "SushiGame/Customer/CustomerProfile")]
public class CustomerProfile : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _id;
    [SerializeField] private string _displayName;
    [SerializeField] private CustomerType _customerType = CustomerType.Normal;

    [Header("Visuals")]
    // 감정과 스프라이트를 짝지을 구조체
    [SerializeField] private List<EmotionSprite> _emotionSprites = new List<EmotionSprite>();

    [Header("Spawn Settings")]
    [SerializeField] private int _minDay = 1;
    [SerializeField] private int _maxDay = 999;
    [SerializeField] private float _spawnWeight = 1f;
    [SerializeField] private bool _uniquePerDay = false;

    [Header("Order Behaviour")]
    [SerializeField] private bool _canMakeCustomOrders = true;
    [SerializeField] private float _customOrderProbability = 0.5f;
    [SerializeField] private List<CustomerOrderPreset> _orderPresets = new List<CustomerOrderPreset>();

    // Properties
    public string Id => _id;
    public string DisplayName => _displayName;
    public CustomerType CustomerType => _customerType;

    public int MinDay => _minDay;
    public int MaxDay => _maxDay;
    public float SpawnWeight => _spawnWeight;
    public bool UniquePerDay => _uniquePerDay;

    public bool CanMakeCustomOrders => _canMakeCustomOrders;
    public float CustomOrderProbability => _customOrderProbability;
    public IReadOnlyList<CustomerOrderPreset> OrderPresets => _orderPresets;

    // --- 핵심 기능: 감정에 맞는 스프라이트 찾아오기 ---
    public Sprite GetSprite(CustomerEmotion emotion)
    {
        // 리스트에서 해당 감정을 찾음
        foreach (var item in _emotionSprites)
        {
            if (item.Emotion == emotion)
            {
                return item.Sprite;
            }
        }

        // 못 찾으면 Default 반환, 그것도 없으면 null
        if (emotion != CustomerEmotion.Default)
        {
            return GetSprite(CustomerEmotion.Default);
        }

        return null;
    }

    // 인스펙터 설정을 위한 내부 클래스
    [System.Serializable]
    public class EmotionSprite
    {
        public CustomerEmotion Emotion;
        public Sprite Sprite;
    }
}

// [CustomerProfile.cs 파일 맨 아래에 추가]

[System.Serializable]
public class CustomerOrderPreset
{
    [Header("Order Identity")]
    [Tooltip("스토리 조건 체크용 ID (예: order_police_first)")]
    [SerializeField] private string _orderId;

    [Header("Base Recipe")]
    [Tooltip("손님이 주문할 기본 레시피")]
    [SerializeField] private SushiRecipe _baseRecipe;

    [Header("Optional Custom Request")]
    [Tooltip("특별 요청 사항 (없으면 비워도 됨)")]
    [SerializeField] private SushiCustomRequest _customRequest;

    [Header("Weight")]
    [Tooltip("여러 프리셋 중 선택될 확률 가중치 (높을수록 자주 주문)")]
    [SerializeField] private float _weight = 1f;

    // 외부에서 데이터를 읽기 위한 프로퍼티
    public string OrderId => _orderId;
    public SushiRecipe BaseRecipe => _baseRecipe;
    public SushiCustomRequest CustomRequest => _customRequest;
    public float Weight => _weight;
}