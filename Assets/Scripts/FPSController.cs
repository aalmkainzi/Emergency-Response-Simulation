using System.Diagnostics;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class FPSController : MonoBehaviour
{
    public float speed = 5f;
    public float mouse_speed = 100f;
    private float x_rot = 0f;

    Camera cam;
    Animator anim;

    Rigidbody rb;

    public GameObject missionSite;
    public GameObject cutscene;
    public GameObject lookSphere;

    int cityLayer;
    private void OnEnable()
    {
        cutscene.SetActive(false);
        missionSite.SetActive(true);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
    }

    void Start()
    {
        cityLayer = LayerMask.NameToLayer("City");
        UnityEngine.Debug.Log("City layer = " + cityLayer);
        rb = GetComponent<Rigidbody>();
        cam = transform.Find("PlayerCamera").GetComponent<Camera>();
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouse_speed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouse_speed * Time.deltaTime;

        x_rot -= mouseY;

        x_rot = Mathf.Clamp(x_rot, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(x_rot, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        float move_h = Input.GetAxis("Horizontal");
        float move_v = Input.GetAxis("Vertical");

        Vector3 velocity = Vector3.zero;
        Vector3 right = transform.right;
        Vector3 fwd = transform.forward;
        velocity += (move_h * speed) * right;
        velocity += (move_v * speed) * fwd;

        if(move_h != 0.0f || move_v != 0.0f)
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);
        }

        rb.linearVelocity= velocity;

        Ray ray = new();
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, maxDistance: 8.5f, layerMask: 1 << cityLayer);

        if(hit)
        {
            Vector3 hitPoint = hitInfo.point;
            UnityEngine.Debug.Log("Ray hit at " + hitPoint);

            lookSphere.transform.position = hitPoint;
        }
        else
        {
            lookSphere.transform.position = new Vector3(-100, -100, -100);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Gizmos.DrawRay(ray);
    }

    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("Player collided with " + collision.gameObject.name);
    }
}