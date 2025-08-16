using UnityEngine;
using UnityEngine.AI;

public class Medic : MonoBehaviour
{
    bool selectedByPlayer = false;
    bool hoveredOver = false;
    bool hoveredOverPrevFrame = false;
    NavMeshAgent agent;

    GameObject hoverShape;
    GameObject selectShape;

    Vector3 currentDestination;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        hoverShape = transform.Find("HoverShape").gameObject;
        selectShape = transform.Find("SelectShape").gameObject;
        agent = GetComponent<NavMeshAgent>();

        currentDestination = transform.position;
    }

    private void Update()
    {
        if (!selectedByPlayer)
        {
            if (hoveredOver && !hoveredOverPrevFrame)
            {
                hoverShape.SetActive(true);
            }
            else if (!hoveredOver && hoveredOverPrevFrame)
            {
                hoverShape.SetActive(false);
            }
        }

        if (hoveredOver)
        {
            hoveredOverPrevFrame = true;
        }
        else
        {
            hoveredOverPrevFrame = false;
        }

        if(Vector3.Distance(currentDestination, transform.position) < 0.1f)
        {
            currentDestination = transform.position;
            agent.ResetPath();

            anim.SetBool("Running", false);
        }
    }

    public void Select()
    {
        selectedByPlayer = true;

        // set some material here? so its visually obvious this one is selected

        hoverShape.SetActive(false);
        selectShape.SetActive(true);
    }

    public void Unselect()
    {
        selectedByPlayer = false;

        selectShape.SetActive(false);
    }

    public void Hover()
    {
        hoveredOver = true;
    }

    public void Unhover()
    {
        hoveredOver = false;
    }

    public void GotoPoint(Vector3 point)
    {
        Debug.Log("agent active = " + agent.isActiveAndEnabled);
        Debug.Log("agent on navmesh = " + agent.isOnNavMesh);

        currentDestination = point;
        agent.SetDestination(point);

        anim.SetBool("Running", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Flammable flammable;
        bool found = other.gameObject.TryGetComponent(out flammable);
        if (found)
        {
            if (flammable.onFire)
            {
                // the medic is on fire. just lose here?
            }
        }
    }
}
