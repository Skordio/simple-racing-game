using UnityEngine;
using System;
using System.Collections.Generic;
using ControlType = SettingsMenuController.ControlType;
using Unity.IO.LowLevel.Unsafe;

public interface InputHandler
{
    ControlType controlType { get; }
    // Adjusts the velocity and rotationSpeed variables in the carController class based on input
    (float moveInput, float turnInput, bool driftInput) HandleInput();
}

public class CarController : MonoBehaviour
{
    private class ArrowKeyMovementHandler : InputHandler
    {
        private CarController Car;

        public ArrowKeyMovementHandler(CarController carController)
        {
            Car = carController;
        }

        public ControlType controlType => ControlType.Keyboard;

        public (float moveInput, float turnInput, bool driftInput) HandleInput()
        {
            float moveInput = 0f;
            float turnInput = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
                moveInput += 1f;
            if (Input.GetKey(KeyCode.DownArrow))
                moveInput -= 0.8f;

            if (Input.GetKey(KeyCode.LeftArrow))
                turnInput += 1f;
            if (Input.GetKey(KeyCode.RightArrow))
                turnInput -= 1f;

            return (moveInput, turnInput, Input.GetKey(KeyCode.Space));
        }
    }

    private class MouseMovementHandler : InputHandler
    {
        private CarController Car;

        public MouseMovementHandler(CarController carController)
        {
            Car = carController;
        }

        public ControlType controlType => ControlType.Mouse;

        public (float moveInput, float turnInput, bool driftInput) HandleInput()
        {

            float turnInput = 0f;
            float moveInput = 0f;
            
            if (Input.GetMouseButton(0))
            {
                // Turning logic
                Vector2 mousePosition = Input.mousePosition;
                Vector2 carPosition = Camera.main.WorldToScreenPoint(Car.transform.position);

                Vector2 directionToMouse = (mousePosition - carPosition).normalized;
                Vector2 carForward = Car.transform.up;

                float angleToMouse = Vector2.SignedAngle(carForward, directionToMouse);
                if ((angleToMouse > -15 && angleToMouse < 15) || angleToMouse > 115 || angleToMouse < -115)
                {
                    turnInput = Mathf.Clamp(angleToMouse, -0.8f, 0.8f);
                }
                else
                {
                    turnInput = angleToMouse / Math.Abs(angleToMouse);
                }

                // Forwards and Backwards acceleration
                if (angleToMouse > 120 || angleToMouse < -120)
                {
                    turnInput = -turnInput;
                    moveInput -= 0.8f;
                }
                else
                {
                    moveInput += 1f;
                }
            }

            return (moveInput, turnInput, Input.GetKey(KeyCode.Space));
        }
    }

    public GameObject settingsMenu;

    public static float MAX_SPEED = 6.5f;
    public static float ACCELERATION = 6f;
    public static float DECELERATION = 3f;
    public static float ROTATION_SPEED = 100f;
    private static int MAX_RESIDUAL_DRIFT_FRAMES = 40;

    private Rigidbody2D rb;
    public Vector2 velocity = Vector2.zero;
    private float rotationSpeed = 0f;
    private int residualDriftFrames = 0;

    private Dictionary<ControlType, InputHandler> inputHandlers = new Dictionary<ControlType, InputHandler>();
    private InputHandler ih;

    private void Awake()
    {
        inputHandlers[ControlType.Keyboard] = new ArrowKeyMovementHandler(this);
        inputHandlers[ControlType.Mouse] = new MouseMovementHandler(this);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ih = new MouseMovementHandler(this);
    }

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
        if (!Mathf.Approximately(velocity.magnitude, 0f))
        {
            rb.angularVelocity = rotationSpeed;
        }
    }

    void Update()
    {
        if (SettingsMenuController.currentControlType != ih.controlType)
            ih = inputHandlers[SettingsMenuController.currentControlType];

        if (settingsMenu.activeSelf)
        {
            rb.simulated = false;
            return;
        }
        else
        {
            rb.simulated = true;
        }

        var (moveInput, turnInput, driftInput) = ih.HandleInput();

        float extraAccelerationFromDrifting = driftInput ? 0 : residualDriftFrames / 10;
        Vector2 accelerationVector = transform.up * moveInput * (ACCELERATION + extraAccelerationFromDrifting);
        velocity += accelerationVector * Time.deltaTime;

        rotationSpeed = turnInput * turnRatioForSpeed(velocity.magnitude) * ROTATION_SPEED;

        if (velocity.magnitude > MAX_SPEED)
            velocity = velocity.normalized * MAX_SPEED;

        if (Mathf.Approximately(moveInput, 0f))
            decelerate();
    }

    void FixedUpdate()
    {
        if (settingsMenu.activeSelf)
            return;

        ApplyMovement();
    }
}
