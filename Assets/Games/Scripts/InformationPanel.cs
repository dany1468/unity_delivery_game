using System.Linq;
using TMPro;
using UnityEngine;

public class InformationPanel : MonoBehaviour
{
    [Header("受け取った食品の UI オブジェクト")]
    [SerializeField] private TextMeshProUGUI[] receivedFoodsUI;

    [SerializeField] private TextMeshProUGUI scoreNumberOfReceived;
    [SerializeField] private TextMeshProUGUI scoreNumberOfDelivered;

    public void ShowReceivedFood(FoodContainer foodContainer)
    {
        var container = foodContainer.CurrentFoods.Select(food => food.FoodName).Concat(new[] { "", "", "" });
        foreach (var (text, foodName) in receivedFoodsUI.Zip(container, (f, rf) => (f, rf)))
        {
            text.SetText(foodName);
        }
    }

    public void UpdateScore(Score score)
    {
        scoreNumberOfReceived.SetText(score.TotalReceivedFoodCount.ToString());
        scoreNumberOfDelivered.SetText(score.TotalDeliveredFoodCount.ToString());
    }
    
    public void Clear()
    {
        foreach (var text in receivedFoodsUI)
        {
            text.SetText(string.Empty);
        }
    }
}