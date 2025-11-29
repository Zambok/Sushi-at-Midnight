using UnityEngine;

public class SushiCraftController : MonoBehaviour
{
    [SerializeField] private SushiRecipe[] _craftableRecipes;

    private SushiRecipe _currentRecipe;
    private SushiProcessParameters _currentParameters;
    private bool _isCrafting;

    public bool IsCrafting => _isCrafting;
    public SushiRecipe CurrentRecipe => _currentRecipe;
    public SushiProcessParameters CurrentParameters => _currentParameters;

    private void Awake()
    {
        _currentParameters = new SushiProcessParameters();
    }

    public void StartCraft(SushiRecipe recipe)
    {
        if (recipe == null)
        {
            return;
        }

        _currentRecipe = recipe;
        ResetParameters();
        _isCrafting = true;
    }

    public void StartCraftRandom()
    {
        if (_craftableRecipes == null || _craftableRecipes.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, _craftableRecipes.Length);
        StartCraft(_craftableRecipes[index]);
    }

    private void ResetParameters()
    {
        if (_currentParameters == null)
        {
            _currentParameters = new SushiProcessParameters();
        }

        _currentParameters.FishThickness = 0.5f;
        _currentParameters.RiceAmount = 0.5f;
        _currentParameters.PressDuration = 0.5f;
    }

    public void SetFishThickness(float normalizedValue)
    {
        if (_isCrafting == false)
        {
            return;
        }

        _currentParameters.FishThickness = Mathf.Clamp01(normalizedValue);
    }

    public void SetRiceAmount(float normalizedValue)
    {
        if (_isCrafting == false)
        {
            return;
        }

        _currentParameters.RiceAmount = Mathf.Clamp01(normalizedValue);
    }

    public void SetPressDuration(float normalizedValue)
    {
        if (_isCrafting == false)
        {
            return;
        }

        _currentParameters.PressDuration = Mathf.Clamp01(normalizedValue);
    }

    public SushiPlate CompleteCraft()
    {
        if (_isCrafting == false || _currentRecipe == null)
        {
            return null;
        }

        SushiProcessParameters copiedParameters = new SushiProcessParameters
        {
            FishThickness = _currentParameters.FishThickness,
            RiceAmount = _currentParameters.RiceAmount,
            PressDuration = _currentParameters.PressDuration
        };

        SushiPlate plate = new SushiPlate(_currentRecipe, copiedParameters);

        _isCrafting = false;
        _currentRecipe = null;

        return plate;
    }

    public void CancelCraft()
    {
        _isCrafting = false;
        _currentRecipe = null;
    }
}
