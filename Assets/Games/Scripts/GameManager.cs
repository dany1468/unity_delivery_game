using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

public class FoodContainer
{
    private readonly List<Food> foods = Enumerable.Empty<Food>().ToList();
    public Food[] CurrentFoods => foods.ToArray();

    public void Put(Food food)
    {
        foods.Add(food);
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
    private int totalReceivedFoodCount = 0;
    private int totalDeliveredFoodCount = 0;
    
    public void AddReceivedFoodCount(int count)
    {
        totalReceivedFoodCount += count;
    }
    
    public void AddDeliveredFoodCount(int count)
    {
        totalDeliveredFoodCount += count;
    }
}

public class GameManager : Singleton<GameManager>
{
    private CarController playerCar;

    [Header("Timer オブジェクト")] [SerializeField]
    private Timer timer;

    public bool GameIsPlaying { get; private set; } = true;

    // TODO: Tag から自動取得でいいかもしれない
    [Header("Provider オブジェクト")] [SerializeField]
    private ProviderController[] providerControllers;

    [SerializeField] private ShokudoController[] ShokudoControllers;

    [Header("全体マップ UI オブジェクト")] [SerializeField]
    private GameObject overallMapUI;

    [SerializeField] private InformationPanel informationPanel;

    private readonly FoodContainer foodContainer = new FoodContainer();
    private readonly Score score = new Score();

    private ProviderController PickAProviderRandomly() =>
        providerControllers[Random.Range(0, providerControllers.Length)];

    private ShokudoController PickAShokudoRandomly() =>
        ShokudoControllers[Random.Range(0, ShokudoControllers.Length)];

    public void ReceiveFood(Food food)
    {
        foodContainer.Put(food);
        informationPanel.ShowReceivedFood(foodContainer);
        score.AddReceivedFoodCount(1);
    }

    public Food[] DeliverFood()
    {
        var deliveredFoods = foodContainer.TakeAll();
        score.AddDeliveredFoodCount(deliveredFoods.Length);
        return deliveredFoods;
    }

    private void Start()
    {
        playerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        ClearReceivedFoods();

        // TODO: とりあえず起動から 1 秒後にスタートにしている
        Invoke(nameof(StartGame), 1.0f);
    }

    private void StartGame()
    {
        playerCar.SetState(CarController.State.Driving);
        timer.StartClock();

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
        overallMapUI.SetActive(playerCar.ShowingMap);
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
    }
}