using System.Diagnostics;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class FPSController : MonoBehaviour
{
    public LayerMask clickable;
    
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

    Medic currentlySelectedMedic = null;
    Medic currentlyHoveredMedic = null;

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

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, maxDistance: 8.5f, layerMask: clickable);

        bool unhover = true;
        if(hit)
        {
            GameObject collidedWith = hitInfo.collider.gameObject;
            if(collidedWith.layer == LayerMask.NameToLayer("City"))
            {
                Vector3 hitPoint = hitInfo.point;

                lookSphere.transform.position = hitPoint;
            }
            else
            {
                Medic medic = collidedWith.GetComponent<Medic>();
                UnityEngine.Debug.Assert(medic != null);
                currentlyHoveredMedic = medic;
                currentlyHoveredMedic.Hover();
                unhover = false;
                lookSphere.transform.position = new Vector3(-100, -100, -100);
            }
        }
        else
        {
            lookSphere.transform.position = new Vector3(-100, -100, -100);
        }

        if(unhover && currentlyHoveredMedic != null)
        {
            currentlyHoveredMedic.Unhover();
            currentlyHoveredMedic = null;
        }

        if (Input.GetMouseButtonDown(0) && currentlyHoveredMedic != null)
        {
            if (currentlySelectedMedic != null)
            {
                currentlySelectedMedic.Unselect();
            }
            currentlySelectedMedic = currentlyHoveredMedic;
            currentlySelectedMedic.Unhover();
            currentlySelectedMedic.Select();
            currentlyHoveredMedic = null;
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