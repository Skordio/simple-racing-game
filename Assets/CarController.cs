using UnityEngine;

public class CarController : MonoBehaviour
{
    public static float MAX_SPEED = 10f;
    public static float MAX_BACKWARD_SPEED = 5f;
    public static float ACCELERATION = 5f;
    public static float DECELERATION = 3f;
    public static float MAX_ROTATION_SPEED = 100f;

    private float speed = 0f;
    private float rotationSpeed = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        // Forward and backward movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveInput = ACCELERATION;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveInput = -ACCELERATION;
        }

        // Left and right turning
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnInput = 1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnInput = -1f;
        }

        // Apply acceleration or deceleration
        if (moveInput != 0)
        {
            speed += moveInput * Time.deltaTime;
            if (speed > MAX_SPEED) speed = MAX_SPEED;
            if (speed < -MAX_BACKWARD_SPEED) speed = -MAX_BACKWARD_SPEED;
        }
        else
        {
            // Deceleration when no input is given
            if (speed > 0)
            {
                speed -= DECELERATION * Time.deltaTime;
                if (speed < 0) speed = 0;
            }
            else if (speed < 0)
            {
                speed += DECELERATION * Time.deltaTime;
                if (speed > 0) speed = 0;
            }
        }

        // Apply rotation
        rotationSpeed = turnInput * MAX_ROTATION_SPEED;
    }

    private void ApplyMovement()
    {
        // Move the car in its current forward direction
        rb.velocity = transform.up * speed;

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
