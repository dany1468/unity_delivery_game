using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProviderController : MonoBehaviour
{
    // NOTE 状態の一覧は仮
    public enum State
    {
        NotProviding,

        // 受け取り可能
        Providing,

        // 期限切れ
        Outdated,

        // 閉まった
        Closing,
    }

    public State CurrentState { get; private set; } = State.NotProviding;

    private ParticleSystem particle;
    private EventController eventController;
    public GameObject[] foods;

    private GameObject providingFood;

    private Fungus.Flowchart flowchart;
    [SerializeField] private string sendMessage = "on_provider_reached";
    [SerializeField] private string sendMessageWhenContainerIsFull = "on_provider_reached_when_container_is_full";

    private void Start()
    {
        flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        particle = GetComponentInChildren<ParticleSystem>();
        StopParticle();
        
        eventController = GetComponentInChildren<EventController>();
        eventController.enabled = false;
        eventController.OnEventTriggered += () =>
        {
            // NOTE コンテナが満タンの場合は食品を受け取れない
            if (GameManager.instance.IsContainerFull())
            {
                StartCoroutine(Talk(GameManager.instance.PlayerCar, sendMessageWhenContainerIsFull));
            }
            else
            {
                StartCoroutine(Talk(GameManager.instance.PlayerCar, sendMessage));

                GameManager.instance.ReceiveFood(providingFood.GetComponent<Food>());
                SetState(State.Closing);
                eventController.enabled = false;
            }
        };
    }

    private GameObject PickAFoodRandomly() => foods[Random.Range(0, foods.Length)];

    public void SetState(State newState)
    {
        switch (newState)
        {
            case State.NotProviding:
                break;
            case State.Providing:
                CurrentState = newState;
                ChangeStateToProviding();
                break;
            case State.Outdated:
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
        var t = transform.position;
        t.y += 5;

        providingFood = Instantiate(PickAFoodRandomly(), t, transform.rotation);

        PlayParticle();

        eventController.enabled = true;
    }

    private void ChangeStateToClosing()
    {
        Destroy(providingFood);
        providingFood = null;

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