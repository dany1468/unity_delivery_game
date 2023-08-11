using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    private Fungus.Flowchart flowchart;
    [SerializeField] private string sendMessage;

    public event System.Action OnEventTriggered;

    private void Start()
    {
        flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        // NOTE: enabled の状態に関わらず OnTrigger は呼ばれてしまうためチェックする
        if (!enabled) return;
        
        if (other.CompareTag("Player"))
        {
            var car = other.GetComponent<CarController>();
            StartCoroutine(Talk(car));
        }
    }

    IEnumerator Talk(CarController player)
    {
        player.SetState(CarController.State.Talking);

        flowchart.SendFungusMessage(sendMessage);
        yield return new WaitUntil(() => flowchart.GetExecutingBlocks().Count == 0);

        player.SetState(CarController.State.Driving);
        OnEventTriggered?.Invoke();
    }
}