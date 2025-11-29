using UnityEngine;

public static class SushiQualityEvaluator
{
    public static float CalculateQualityScore(
        SushiRecipe recipe,
        SushiProcessParameters actual)
    {
        return CalculateQualityScore(recipe, actual, null);
    }

    public static float CalculateQualityScore(
        SushiRecipe recipe,
        SushiProcessParameters actual,
        SushiCustomRequest customRequest)
    {
        if (recipe == null || actual == null)
        {
            return 0f;
        }

        SushiProcessParameters ideal = CloneParameters(recipe.IdealParameters);
        SushiProcessParameters tolerance = recipe.ToleranceParameters;

        ApplyCustomRequestToIdeal(ideal, customRequest);

        float fishScore = CalculateOneScore(actual.FishThickness, ideal.FishThickness, tolerance.FishThickness);
        float riceScore = CalculateOneScore(actual.RiceAmount, ideal.RiceAmount, tolerance.RiceAmount);
        float pressScore = CalculateOneScore(actual.PressDuration, ideal.PressDuration, tolerance.PressDuration);
        float wasabiScore = CalculateOneScore(actual.WasabiAmount, ideal.WasabiAmount, tolerance.WasabiAmount);

        float totalWeight =
            recipe.FishThicknessWeight +
            recipe.RiceAmountWeight +
            recipe.PressDurationWeight +
            recipe.WasabiAmountWeight;

        if (totalWeight <= 0f)
        {
            return 0f;
        }

        float weightedScore =
            fishScore * recipe.FishThicknessWeight +
            riceScore * recipe.RiceAmountWeight +
            pressScore * recipe.PressDurationWeight +
            wasabiScore * recipe.WasabiAmountWeight;

        float normalizedScore = weightedScore / totalWeight;
        return Mathf.Clamp01(normalizedScore);
    }

    private static SushiProcessParameters CloneParameters(SushiProcessParameters source)
    {
        if (source == null)
        {
            return new SushiProcessParameters();
        }

        return new SushiProcessParameters
        {
            FishThickness = source.FishThickness,
            RiceAmount = source.RiceAmount,
            PressDuration = source.PressDuration,
            WasabiAmount = source.WasabiAmount
        };
    }

    private static void ApplyCustomRequestToIdeal(
        SushiProcessParameters ideal,
        SushiCustomRequest customRequest)
    {
        if (ideal == null || customRequest == null || customRequest.HasAnyRequest == false)
        {
            return;
        }

        if (customRequest.HasRiceAmountRequest)
        {
            ideal.RiceAmount = Mathf.Clamp01(ideal.RiceAmount + customRequest.RiceAmountOffset);
        }

        if (customRequest.HasFishThicknessRequest)
        {
            ideal.FishThickness = Mathf.Clamp01(ideal.FishThickness + customRequest.FishThicknessOffset);
        }

        if (customRequest.HasPressDurationRequest)
        {
            ideal.PressDuration = Mathf.Clamp01(ideal.PressDuration + customRequest.PressDurationOffset);
        }

        if (customRequest.HasWasabiAmountRequest)
        {
            ideal.WasabiAmount = Mathf.Clamp01(ideal.WasabiAmount + customRequest.WasabiAmountOffset);
        }
    }

    private static float CalculateOneScore(float actual, float ideal, float tolerance)
    {
        float diff = Mathf.Abs(actual - ideal);

        if (tolerance <= 0f)
        {
            return diff == 0f ? 1f : 0f;
        }

        float ratio = diff / tolerance;
        float score = 1f - ratio;

        return Mathf.Clamp01(score);
    }

    public static OrderResult ToOrderResult(float qualityScore)
    {
        if (qualityScore >= 0.9f)
        {
            return OrderResult.Perfect;
        }

        if (qualityScore >= 0.7f)
        {
            return OrderResult.Good;
        }

        return OrderResult.Fail;
    }
}
