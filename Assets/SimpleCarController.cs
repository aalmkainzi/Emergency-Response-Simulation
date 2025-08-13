using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public float accelerationForce;
    public float breakingForce;
    public float maxSpeed;
    public float turnStrength;
    public float sidewaysGrip;
    public float drag;

    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;

    private AudioSource sirenAudioSource;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sirenAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!sirenAudioSource.isPlaying)
                sirenAudioSource.Play();
            else
                sirenAudioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
        ApplyFriction();
    }

    void HandleMovement()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (forwardSpeed < maxSpeed && verticalInput > 0)
        {
            rb.AddForce(transform.forward * verticalInput * accelerationForce);
        }

        if (verticalInput < 0)
        {
            rb.AddForce(transform.forward * verticalInput * breakingForce);
        }
    }

    void HandleSteering()
    {
        float turnMultiplier = 0.5f; // Mathf.Clamp01(rb.linearVelocity.magnitude /maxSpeed);

        rb.AddTorque(transform.up * horizontalInput * turnStrength * turnMultiplier);
    }

    void ApplyFriction()
    {
        Vector3 sidewaysVelocity = Vector3.Project(rb.linearVelocity, transform.right);

        rb.AddForce(-sidewaysVelocity * sidewaysGrip);

        if (Mathf.Abs(verticalInput) < 0.1f)
        {
            Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
            rb.AddForce(-forwardVelocity * drag);
        }
    }
}
