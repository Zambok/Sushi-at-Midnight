using UnityEngine;

public class Order
{
    public SushiRecipe BaseRecipe { get; private set; }
    public SushiCustomRequest CustomRequest { get; private set; }
    public Customer Owner { get; private set; }

    public float TimeLimit { get; private set; }
    public float RemainingTime { get; private set; }

    public OrderState State { get; private set; } = OrderState.None;

    public Order(
        SushiRecipe baseRecipe,
        SushiCustomRequest customRequest,
        Customer owner,
        float timeLimit)
    {
        BaseRecipe = baseRecipe;
        CustomRequest = customRequest;
        Owner = owner;
        TimeLimit = timeLimit;
        RemainingTime = timeLimit;
        State = OrderState.Active;
    }

    public void Tick(float deltaTime)
    {
        if (State != OrderState.Active)
        {
            return;
        }

        RemainingTime -= deltaTime;

        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            Fail();
        }
    }

    public OrderResult Complete(SushiPlate sushiPlate, out float qualityScore)
    {
        qualityScore = 0f;

        if (State != OrderState.Active)
        {
            return OrderResult.None;
        }

        if (sushiPlate == null || sushiPlate.TargetRecipe != BaseRecipe)
        {
            State = OrderState.Failed;
            return OrderResult.Fail;
        }

        qualityScore = SushiQualityEvaluator.CalculateQualityScore(
            BaseRecipe,
            sushiPlate.ActualParameters,
            CustomRequest
        );

        OrderResult result = SushiQualityEvaluator.ToOrderResult(qualityScore);
        State = OrderState.Completed;

        return result;
    }

    public void Fail()
    {
        if (State != OrderState.Active)
        {
            return;
        }

        State = OrderState.Failed;
    }
}
