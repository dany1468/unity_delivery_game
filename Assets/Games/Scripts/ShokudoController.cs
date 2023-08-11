using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShokudoController: MonoBehaviour
{
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
    
    private void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        eventController = GetComponentInChildren<EventController>();
        eventController.enabled = false;
        eventController.OnEventTriggered += () =>
        {
            var deliveredFoods = GameManager.instance.DeliverFood();
            SetState(State.Received);
            eventController.enabled = false;
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
        var t = transform.position;
        t.y += 5;
        
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
}