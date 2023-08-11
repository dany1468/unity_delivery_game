using System;
using System.Collections;
using UnityEngine;

public class ShokudoController : MonoBehaviour
{
    // NOTE 状態の一覧は仮
    public enum State
    {
        NotReceiving,

        // 受け取り可能
        Receiving,

        // 受け取り済 
        Received,

        // 閉まった
        Closing,
    }

    public State CurrentState { get; private set; } = State.NotReceiving;

    private ParticleSystem particle;
    private EventController eventController;

    private Fungus.Flowchart flowchart;
    [SerializeField] private string sendMessage = "on_shokudo_reached";
    [SerializeField] private string sendMessageWithoutFood = "on_shokudo_reached_without_food";

    private void Start()
    {
        flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        particle = GetComponentInChildren<ParticleSystem>();
        StopParticle();
        
        eventController = GetComponentInChildren<EventController>();
        eventController.enabled = false;
        eventController.OnEventTriggered += () =>
        {
            // NOTE 食品を受け取っている場合のみ受領状態になる
            if (GameManager.instance.HasFood())
            {
                StartCoroutine(Talk(GameManager.instance.PlayerCar, sendMessage));
                
                var deliveredFoods = GameManager.instance.DeliverFood();
                SetState(State.Received);
                eventController.enabled = false;
            }
            else
            {
                StartCoroutine(Talk(GameManager.instance.PlayerCar, sendMessageWithoutFood));
            }
        };
    }

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.NotReceiving:
                break;
            case State.Receiving:
                CurrentState = newState;
                ChangeStateToProviding();
                break;
            case State.Received:
                CurrentState = newState;
                ChangeStateToReceived();
                break;
            case State.Closing:
                CurrentState = newState;
                ChangeStateToClosing();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void ChangeStateToProviding()
    {
        PlayParticle();

        eventController.enabled = true;
    }

    private void ChangeStateToReceived()
    {
        StopParticle();

        eventController.enabled = false;
    }

    private void ChangeStateToClosing()
    {
        StopParticle();

        eventController.enabled = false;
    }

    private void PlayParticle() => particle.Play();

    private void StopParticle() => particle.Stop();

    IEnumerator Talk(CarController player, string message)
    {
        player.SetState(CarController.State.Talking);

        flowchart.SendFungusMessage(message);

        yield return new WaitUntil(() => flowchart.GetExecutingBlocks().Count == 0);

        player.SetState(CarController.State.Driving);
    }
}