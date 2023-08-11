using System.Linq;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreNumberOfReceived;
    [SerializeField] private TextMeshProUGUI scoreNumberOfDelivered;

    public void ShowScore(Score score)
    {
        scoreNumberOfReceived.SetText(score.TotalReceivedFoodCount.ToString());
        scoreNumberOfDelivered.SetText(score.TotalDeliveredFoodCount.ToString());
    }
}