using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class FoodContainer
{
    private const int MaxFoodCount = 3;
        
    private readonly List<Food> foods = Enumerable.Empty<Food>().ToList();
    public Food[] CurrentFoods => foods.ToArray();
    public bool IsFull => MaxFoodCount <= foods.Count;

    public bool Put(Food food)
    {
        if (IsFull) return false;
        
        foods.Add(food);

        return true;
    }

    public Food[] TakeAll()
    {
        var result = CurrentFoods;
        foods.Clear();
        return result;
    }
}

public class Score
{
    public int TotalReceivedFoodCount { get; private set; } = 0;

    public int TotalDeliveredFoodCount { get; private set; } = 0;

    public void AddReceivedFoodCount(int count)
    {
        TotalReceivedFoodCount += count;
    }
    
    public void AddDeliveredFoodCount(int count)
    {
        TotalDeliveredFoodCount += count;
    }
}

public class GameManager : Singleton<GameManager>
{
    public CarController PlayerCar { get; private set; }

    [Header("Timer オブジェクト")] [SerializeField]
    private Timer timer;

    public bool GameIsPlaying { get; private set; } = false;

    // TODO: Tag から自動取得でいいかもしれない
    [Header("Provider オブジェクト")] [SerializeField]
    private ProviderController[] providerControllers;

    [SerializeField] private ShokudoController[] ShokudoControllers;


    [Header("ゲーム中の UI オブジェクト")] [SerializeField]
    private GameObject inGameUI;
    
    [Header("全体マップ UI オブジェクト")] [SerializeField]
    private GameObject overallMapUI;

    [SerializeField] 
    private InformationPanel informationPanel;
    
    [Header("ゲーム終了 UI オブジェクト")] [SerializeField]
    private GameObject gameOverUI;

    private readonly FoodContainer foodContainer = new FoodContainer();
    private readonly Score score = new Score();

    private ProviderController PickAProviderRandomly() =>
        providerControllers[Random.Range(0, providerControllers.Length)];

    private ShokudoController PickAShokudoRandomly() =>
        ShokudoControllers[Random.Range(0, ShokudoControllers.Length)];

    public void ReceiveFood(Food food)
    {
        foodContainer.Put(food);
        score.AddReceivedFoodCount(1);
        UpdateInformationPanel();
    }

    public Food[] DeliverFood()
    {
        var deliveredFoods = foodContainer.TakeAll();
        score.AddDeliveredFoodCount(deliveredFoods.Length);
        UpdateInformationPanel();
        return deliveredFoods;
    }
    
    public bool HasFood() => foodContainer.CurrentFoods.Any();
    public bool IsContainerFull() => foodContainer.IsFull;
    
    private void UpdateInformationPanel()
    {
        informationPanel.ShowReceivedFood(foodContainer);
        informationPanel.UpdateScore(score);
    }

    private void Start()
    {
        inGameUI.SetActive(false);
        gameOverUI.SetActive(false);
        
        PlayerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        ClearReceivedFoods();
        GameIsPlaying = true;

        // TODO: とりあえず起動から 1 秒後にスタートにしている
        Invoke(nameof(StartGame), 1.0f);
    }
    
    private void StartGame()
    {
        PlayerCar.SetState(CarController.State.Driving);
        timer.StartClock();
        inGameUI.SetActive(true);
        gameOverUI.SetActive(false);

        Invoke(nameof(ActivateProviderRandomly), 5.0f);
        Invoke(nameof(ActivateShokudoRandomly), 5.0f);
    }

    private void ClearReceivedFoods()
    {
        informationPanel.Clear();
    }

    private void Update()
    {
        // NOTE: 全体マップ表示の切り替え
        overallMapUI.SetActive(PlayerCar.ShowingMap);
    }

    private void ActivateProviderRandomly()
    {
        var hasProvidingState = providerControllers.Any(pc => pc.CurrentState == ProviderController.State.Providing);
        // 1 件も供給中の店が無ければ
        if (!hasProvidingState)
        {
            PickAProviderRandomly().SetState(ProviderController.State.Providing);
            Invoke(nameof(ActivateProviderRandomly), 10.0f);
        }
        else
        {
            Invoke(nameof(ActivateProviderRandomly), 1.0f);
        }
    }

    private void ActivateShokudoRandomly()
    {
        var hasProvidingState = ShokudoControllers.Any(pc => pc.CurrentState == ShokudoController.State.Receiving);
        // 1 件も供給中の店が無ければ
        if (!hasProvidingState)
        {
            PickAShokudoRandomly().SetState(ShokudoController.State.Receiving);
            Invoke(nameof(ActivateShokudoRandomly), 10.0f);
        }
        else
        {
            Invoke(nameof(ActivateShokudoRandomly), 1.0f);
        }
    }

    public void GameOver()
    {
        PlayerCar.SetState(CarController.State.Pausing);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }
}