using UnityEngine;
using System;

public interface InputHandler {
    // Adjusts the velocity and rotationSpeed variables in the carController class based on input
    void HandleInput();
}

public class CarController : MonoBehaviour
{
    public static float MAX_SPEED = 6.5f;
    public static float ACCELERATION = 6f;
    public static float DECELERATION = 3f;
    public static float MAX_ROTATION_SPEED = 100f;
    
    private static int MAX_RESIDUAL_DRIFT_FRAMES = 40;

    private class ArrowKeyMovementHandler : InputHandler
    {
        private CarController Car;

        public ArrowKeyMovementHandler(CarController carController)
        {
            Car = carController;
        }

        public void HandleInput()
        {
            float moveInput = 0f;
            float turnInput = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
                moveInput += 1f;
            if (Input.GetKey(KeyCode.DownArrow))
                moveInput -= 0.8f;

            if (Input.GetKey(KeyCode.LeftArrow))
                turnInput += 1f * Car.turnRatioForSpeed(Car.velocity.magnitude);
            if (Input.GetKey(KeyCode.RightArrow))
                turnInput -= 1f * Car.turnRatioForSpeed(Car.velocity.magnitude);

            // Apply acceleration
            float extraAccelerationFromDrifting = Input.GetKey(KeyCode.Space) ? 0 : Car.residualDriftFrames / 10;
            Vector2 accelerationVector = Car.transform.up * moveInput * (ACCELERATION + extraAccelerationFromDrifting);
            Car.velocity += accelerationVector * Time.deltaTime;

            // Deceleration
            if (Mathf.Approximately(moveInput, 0f))
                Car.decelerate();

            // Clamp the overall velocity to a maximum speed
            if (Car.velocity.magnitude > MAX_SPEED)
                Car.velocity = Car.velocity.normalized * MAX_SPEED;

            // Apply rotation input
            Car.rotationSpeed = turnInput * MAX_ROTATION_SPEED;
        }
    }

    private class MouseMovementHandler : InputHandler
    {
        private CarController Car;

        public MouseMovementHandler(CarController carController)
        {
            Car = carController;
        }

        public void HandleInput()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 carPosition = Camera.main.WorldToScreenPoint(Car.transform.position);

            // Create a ray going from the car to the mouse

            // Create a ray indicating the way the car is facing

            // Find the angle between the two rays to determine which way the car should turn, and by how much

            // Hold w to accelerate, s to accelerate backwards, shift to drive slower, and space for drifting
            

        }
    }

    public Vector2 velocity = Vector2.zero;
    private float rotationSpeed = 0f;
    private int residualDriftFrames = 0;

    private float nextDebugLogTime = 0f;

    private InputHandler ih;
    private Rigidbody2D rb;

    // Returns a turning ratio based on the car’s speed.
    private float turnRatioForSpeed(float currentSpeed)
    {
        currentSpeed = Mathf.Abs(currentSpeed);
        float turn_ratio = 0.8f;

        if (currentSpeed <= (MAX_SPEED / 2))
        {
            return currentSpeed * turn_ratio;
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                return currentSpeed * 0.5f;
            }
            else
            {
                float neg_turn_ratio = 0.2f;
                float yIntercept = (MAX_SPEED / 2f) * (turn_ratio + neg_turn_ratio);
                return -(neg_turn_ratio * currentSpeed) + yIntercept;
            }
        }
    }

    private void decelerate()
    {
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, DECELERATION * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        // Check if the difference between car's stored velocity and the rigid body's velocity on screen
        if (Math.Abs((rb.linearVelocity - velocity).magnitude) > 1f)
            velocity = Vector2.zero;

        if (residualDriftFrames > 0)
            residualDriftFrames--;

        // Decompose the current velocity into forward and right components.
        Vector2 forward = transform.up;
        Vector2 right = transform.right;
        float forwardComponent = Vector2.Dot(velocity, forward);
        float lateralComponent = Vector2.Dot(velocity, right);

        float driftLateralDamping = 5f;
        float noDriftLateralDamping = 5f + ((MAX_RESIDUAL_DRIFT_FRAMES - residualDriftFrames) * 3);

        // Apply lateral dampening
        float lateralDamping = Input.GetKey(KeyCode.Space) ? driftLateralDamping : noDriftLateralDamping;
        lateralComponent = Mathf.Lerp(lateralComponent, 0f, lateralDamping * Time.deltaTime);

        if (lateralComponent > 0.2f)
        {
            residualDriftFrames = MAX_RESIDUAL_DRIFT_FRAMES;
        }

        // Reconstruct the velocity vector from its forward and lateral parts.
        velocity = (forward * forwardComponent) + (right * lateralComponent);

        rb.linearVelocity = velocity;
        rb.angularVelocity = rotationSpeed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ih = new MouseMovementHandler(this);
    }

    void Update()
    {
        ih.HandleInput();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }
}
