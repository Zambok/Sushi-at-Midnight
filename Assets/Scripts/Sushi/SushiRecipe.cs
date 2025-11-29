using UnityEngine;

[CreateAssetMenu(fileName = "SushiRecipe", menuName = "Sushi/SushiRecipe")]
public class SushiRecipe : ScriptableObject
{
    [SerializeField] private SushiType _sushiType;

    [Header("Ideal Process Values (0 ~ 1)")]
    [SerializeField] private SushiProcessParameters _idealParameters;

    [Header("Allowed Tolerance")]
    [SerializeField] private SushiProcessParameters _toleranceParameters;

    [Header("Score Weights")]
    [SerializeField] private float _fishThicknessWeight = 1f;
    [SerializeField] private float _riceAmountWeight = 1f;
    [SerializeField] private float _pressDurationWeight = 1f;
    [SerializeField] private float _wasabiAmountWeight = 1f;

    [Header("Base Score")]
    [SerializeField] private int _baseScore = 100;

    public SushiType SushiType => _sushiType;
    public SushiProcessParameters IdealParameters => _idealParameters;
    public SushiProcessParameters ToleranceParameters => _toleranceParameters;

    public float FishThicknessWeight => _fishThicknessWeight;
    public float RiceAmountWeight => _riceAmountWeight;
    public float PressDurationWeight => _pressDurationWeight;
    public float WasabiAmountWeight => _wasabiAmountWeight;

    public int BaseScore => _baseScore;
}
