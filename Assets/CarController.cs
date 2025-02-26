using UnityEngine;

public class CarController : MonoBehaviour
{
    public static float MAX_SPEED = 10f;
    public static float MAX_ROTATION_SPEED = 100f;
    public float speed = 0f;
    public float rotationSpeed = 0f;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
