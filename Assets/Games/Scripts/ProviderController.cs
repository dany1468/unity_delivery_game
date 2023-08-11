using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProviderController : MonoBehaviour
{
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
    
    private void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        eventController = GetComponentInChildren<EventController>();
        eventController.enabled = false;
        eventController.OnEventTriggered += () =>
        {
            GameManager.instance.ReceiveFood(providingFood.GetComponent<Food>());
            SetState(State.Closing);
            eventController.enabled = false;
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
}
