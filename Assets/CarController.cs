using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public interface InputHandler {
    public abstract void HandleInput();
}

public class CarController : MonoBehaviour
{

    private class ArrowKeyMovementHandler : InputHandler
    {
        private CarController Car;

        public ArrowKeyMovementHandler(CarController CarController)
        {
            Car = CarController;
        }

        public void HandleInput()
        {
            float moveInput = 0f;
            float turnInput = 0f;

            // Forward and backward movement
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveInput += ACCELERATION;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space))
            {
                moveInput -= ACCELERATION;
            }

            // Left and right turning
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                turnInput = Car.TurnInputForSpeed();
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                turnInput = -Car.TurnInputForSpeed();
            }

            // Apply acceleration or deceleration
            if (moveInput != 0)
            {
                Car.speed += moveInput * Time.deltaTime;
                if (Car.speed > MAX_SPEED) Car.speed = MAX_SPEED;
                if (Car.speed < -MAX_BACKWARD_SPEED) Car.speed = -MAX_BACKWARD_SPEED;
            }
            else
            {
                // Deceleration when no input is given
                if (Car.speed > 0)
                {
                    Car.speed -= DECELERATION * Time.deltaTime;
                    if (Car.speed < 0) Car.speed = 0;
                }
                else if (Car.speed < 0)
                {
                    Car.speed += DECELERATION * Time.deltaTime;
                    if (Car.speed > 0) Car.speed = 0;
                }
            }

            // Apply rotation
            Car.rotationSpeed = turnInput * MAX_ROTATION_SPEED;
        }
    }

    public static float MAX_SPEED = 7f;
    public static float MAX_BACKWARD_SPEED = MAX_SPEED / 2;
    public static float ACCELERATION = 6f;
    public static float DECELERATION = 3f;
    public static float MAX_ROTATION_SPEED = 100f;

    private float speed = 0f;
    private float rotationSpeed = 0f;

    private InputHandler ih;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ih = new ArrowKeyMovementHandler(this);
    }

    // Update is called once per frame
    void Update()
    {
        ih.HandleInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private float TurnInputForSpeed()
    {
        float forSpeed = Math.Abs(speed);
        float turn_ratio = 0.8f;
        // Piecewise func for turning speed to act more like real turning
        if (forSpeed >= 0 && forSpeed <= (MAX_SPEED / 2))
        {
            return forSpeed * turn_ratio;
        }
        else
        {
            float neg_turn_ratio = 0.2f;
            float yIntercept = MAX_SPEED / 2f * (turn_ratio + neg_turn_ratio);

            return -(neg_turn_ratio * forSpeed) + yIntercept;
        }

    }

    private void ApplyMovement()
    {
        // Check if car has stopped moving and adjust speed variable appropriately
        if (Math.Abs(Vector2.Dot(rb.linearVelocity, transform.up) - speed) > 0.5f)
        {
            speed = Vector2.Dot(rb.linearVelocity, transform.up);
        }

        // Move the car in its current forward direction
        rb.linearVelocity = transform.up * speed;

        // Rotate the car
        if (speed != 0) // Only turn when moving
        {
            rb.angularVelocity = rotationSpeed;
        }
        else
        {
            rb.angularVelocity = 0;
        }
    }
}
