using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _totalScore;
    [SerializeField] private int _completedOrderCount;
    [SerializeField] private int _failedOrderCount;
    [SerializeField] private float _totalQualityScore;

    public int TotalScore => _totalScore;
    public int CompletedOrderCount => _completedOrderCount;
    public int FailedOrderCount => _failedOrderCount;

    public float AverageQualityScore
    {
        get
        {
            if (_completedOrderCount <= 0)
            {
                return 0f;
            }

            return _totalQualityScore / _completedOrderCount;
        }
    }

    public Action<Customer, CustomerReactionType> OnCustomerReactionEvaluated;

    public void ResetScore()
    {
        _totalScore = 0;
        _completedOrderCount = 0;
        _failedOrderCount = 0;
        _totalQualityScore = 0f;
    }

    public void ApplyOrderResult(
        Customer customer,
        SushiRecipe recipe,
        SushiProcessParameters actualParameters,
        OrderResult result,
        float qualityScore)
    {
        if (recipe == null)
        {
            return;
        }

        int scoreDelta = CalculateScoreDelta(recipe, result, qualityScore);

        _totalScore += scoreDelta;

        if (result == OrderResult.Fail)
        {
            _failedOrderCount++;
        }
        else
        {
            _completedOrderCount++;
            _totalQualityScore += qualityScore;
        }

        CustomerReactionType reaction = EvaluateReaction(result, qualityScore);

        if (customer != null && OnCustomerReactionEvaluated != null)
        {
            OnCustomerReactionEvaluated.Invoke(customer, reaction);
        }
    }

    private int CalculateScoreDelta(SushiRecipe recipe, OrderResult result, float qualityScore)
    {
        int baseScore = recipe.BaseScore;
        float multiplier = 0f;

        switch (result)
        {
            case OrderResult.Perfect:
                multiplier = Mathf.Lerp(0.8f, 1.2f, qualityScore);
                break;
            case OrderResult.Good:
                multiplier = Mathf.Lerp(0.4f, 0.8f, qualityScore);
                break;
            case OrderResult.Fail:
                multiplier = -0.5f;
                break;
            default:
                multiplier = 0f;
                break;
        }

        float rawScore = baseScore * multiplier;
        int finalScore = Mathf.RoundToInt(rawScore);

        return finalScore;
    }

    private CustomerReactionType EvaluateReaction(OrderResult result, float qualityScore)
    {
        if (result == OrderResult.Fail)
        {
            return CustomerReactionType.Angry;
        }

        if (qualityScore >= 0.9f)
        {
            return CustomerReactionType.VeryHappy;
        }

        if (qualityScore >= 0.7f)
        {
            return CustomerReactionType.Happy;
        }

        if (qualityScore >= 0.4f)
        {
            return CustomerReactionType.Neutral;
        }

        return CustomerReactionType.Angry;
    }
}
