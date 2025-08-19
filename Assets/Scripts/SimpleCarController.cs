using UnityEngine;
using UnityEngine.Events;

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
    AudioSource engineAudioSource = null;

    public AudioClip sirenSound;
    public AudioClip drivingSound;
    public AudioClip carEngineSound;

    bool initailized = false;

    public UnityEvent onSiren;
    void Start()
    {

    }

    private void OnEnable()
    {
        if(!initailized)
        {
            rb = GetComponent<Rigidbody>();

            AudioSource[] audioSources = GetComponents<AudioSource>();
            sirenAudioSource = audioSources[0];
            drivingAudioSource = audioSources[1];
            engineAudioSource = audioSources[2];
            engineAudioSource.volume = 0.5f;
            sirenAudioSource.volume = 0.35f;
            sirenAudioSource.spatialBlend = 0.5f;

            sirenAudioSource.clip = sirenSound;
            drivingAudioSource.clip = drivingSound;
            engineAudioSource.clip = carEngineSound;

            drivingAudioSource.Play();

            initailized = true;
        }

        Debug.Log("FIRE TRUCK ON ENABLE");
        engineAudioSource.Play();
    }

    private void OnDisable()
    {
        Debug.Log("FIRE TRUCK ON DISABLE");

        engineAudioSource.Stop();
    }


    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!sirenAudioSource.isPlaying)
            {
                onSiren?.Invoke();
                sirenAudioSource.Play();
            }
            else
            {
                sirenAudioSource.Stop();
            }
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
        
        drivingAudioSource.volume = (forwardSpeed * 4) / maxSpeed;
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
