using System.Linq;
using TMPro;
using UnityEngine;

public class InformationPanel : MonoBehaviour
{
    [Header("受け取った食品の UI オブジェクト")]
    [SerializeField] private TextMeshProUGUI[] receivedFoodsUI;

    public void ShowReceivedFood(FoodContainer foodContainer)
    {
        foreach (var (food, text) in foodContainer.CurrentFoods.Zip(receivedFoodsUI, (f, rf) => (f, rf)))
        {
            text.SetText(food.FoodName);
        }
    }
    
    public void Clear()
    {
        foreach (var text in receivedFoodsUI)
        {
            text.SetText(string.Empty);
        }
    }
}