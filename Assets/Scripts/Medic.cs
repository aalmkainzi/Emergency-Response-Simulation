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
    private void Start()
    {
        hoverShape = transform.Find("HoverShape").gameObject;
        selectShape = transform.Find("SelectShape").gameObject;
        agent = GetComponent<NavMeshAgent>();
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
        agent.SetDestination(point);
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
