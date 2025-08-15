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

    AudioSource sirenAudioSource;
    AudioSource drivingAudioSource;

    public AudioClip sirenSound;
    public AudioClip drivingSound;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        sirenAudioSource = audioSources[0];
        drivingAudioSource = audioSources[1];

        sirenAudioSource.clip = sirenSound;
        drivingAudioSource.clip = drivingSound;
    }

    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!sirenAudioSource.isPlaying)
                sirenAudioSource.Play();
            else
                sirenAudioSource.Stop();
        }
    }

    private void OnDisable()
    {
        sirenAudioSource.Stop();
        drivingAudioSource.Stop();
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

        bool moving = false;
        if (forwardSpeed < maxSpeed && verticalInput > 0)
        {
            moving = true;

            rb.AddForce(transform.forward * verticalInput * accelerationForce);
        }

        if (verticalInput < 0)
        {
            moving = true;
            rb.AddForce(transform.forward * verticalInput * breakingForce);
        }

        if(moving)
        {
            if (!drivingAudioSource.isPlaying)
                drivingAudioSource.Play();
        }
        else
        {
            if (drivingAudioSource.isPlaying)
                drivingAudioSource.Stop();
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
