using UnityEngine;

[System.Serializable]
public class SushiCustomRequest
{
    [Header("Rice")]
    [SerializeField] private bool _hasRiceAmountRequest;
    [SerializeField] private float _riceAmountOffset;

    [Header("Fish")]
    [SerializeField] private bool _hasFishThicknessRequest;
    [SerializeField] private float _fishThicknessOffset;

    [Header("Press")]
    [SerializeField] private bool _hasPressDurationRequest;
    [SerializeField] private float _pressDurationOffset;

    [Header("Wasabi")]
    [SerializeField] private bool _hasWasabiAmountRequest;
    [SerializeField] private float _wasabiAmountOffset;

    public bool HasRiceAmountRequest => _hasRiceAmountRequest;
    public float RiceAmountOffset => _riceAmountOffset;

    public bool HasFishThicknessRequest => _hasFishThicknessRequest;
    public float FishThicknessOffset => _fishThicknessOffset;

    public bool HasPressDurationRequest => _hasPressDurationRequest;
    public float PressDurationOffset => _pressDurationOffset;

    public bool HasWasabiAmountRequest => _hasWasabiAmountRequest;
    public float WasabiAmountOffset => _wasabiAmountOffset;

    public bool HasAnyRequest =>
        _hasRiceAmountRequest ||
        _hasFishThicknessRequest ||
        _hasPressDurationRequest ||
        _hasWasabiAmountRequest;

    public static SushiCustomRequest CreateRiceLess(float offset = -0.3f)
    {
        var request = new SushiCustomRequest
        {
            _hasRiceAmountRequest = true,
            _riceAmountOffset = offset
        };

        return request;
    }

    public static SushiCustomRequest CreateThickFish(float offset = 0.3f)
    {
        var request = new SushiCustomRequest
        {
            _hasFishThicknessRequest = true,
            _fishThicknessOffset = offset
        };

        return request;
    }

    public static SushiCustomRequest CreateMoreWasabi(float offset = 0.4f)
    {
        var request = new SushiCustomRequest
        {
            _hasWasabiAmountRequest = true,
            _wasabiAmountOffset = offset
        };

        return request;
    }

    public static SushiCustomRequest CreateSoftPress(float offset = -0.3f)
    {
        var request = new SushiCustomRequest
        {
            _hasPressDurationRequest = true,
            _pressDurationOffset = offset
        };

        return request;
    }
}
