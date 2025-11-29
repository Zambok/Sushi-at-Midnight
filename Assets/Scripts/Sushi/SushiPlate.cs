public class SushiPlate
{
    public SushiRecipe TargetRecipe { get; private set; }
    public SushiProcessParameters ActualParameters { get; private set; }

    public SushiPlate(SushiRecipe recipe, SushiProcessParameters actualParameters)
    {
        TargetRecipe = recipe;
        ActualParameters = actualParameters;
    }
}
