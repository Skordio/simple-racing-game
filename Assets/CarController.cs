using UnityEngine;

public class CarController : MonoBehaviour
{
    public static float MAX_SPEED = 7f;
    public static float MAX_BACKWARD_SPEED = MAX_SPEED/2;
    public static float ACCELERATION = 6f;
    public static float DECELERATION = 3f;
    public static float MAX_ROTATION_SPEED = 100f;

    private float speed = 0f;
    private float rotationSpeed = 0f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private float TurnInputForSpeed(float speed)
    {
        float turn_ratio = 0.8f;
        if (speed == 0)
        {
            return 0f;
        }
        else if (speed < 0)
        {
            return TurnInputForSpeed(-speed);
        }
        // Piecewise func for turning speed to act more like real turning
        else if (speed > 0 && speed <= (MAX_SPEED / 2))
        {
            return speed * turn_ratio;
        }
        else
        {
            float neg_turn_ratio = 0.2f;
            float yIntercept = (MAX_SPEED / 2f)*(turn_ratio + neg_turn_ratio);
            
            return -(neg_turn_ratio * speed) + yIntercept;
        }
        
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
            turnInput = TurnInputForSpeed(speed);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnInput = -TurnInputForSpeed(speed);
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
