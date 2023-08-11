using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum State
    {
        Pausing,
        Driving,
        Talking,
    }
    
    public bool ShowingMap { get; private set; } = false;

    [SerializeField] private float speed = 10f;

    [SerializeField] private float turnSpeed = 45f;

    private float horizontalInput;
    private float verticalInput;

    private State state = State.Pausing;

    public void SetState(State newState)
    {
        this.state = newState;
    }

    private void Update()
    {
        // NOTE: toggle showing map
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ShowingMap = !ShowingMap;
        }
        
        // NOTE: 運転中
        if (state == State.Driving)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            transform.Translate(Vector3.forward * (Time.deltaTime * speed * verticalInput));
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        }
    }
}