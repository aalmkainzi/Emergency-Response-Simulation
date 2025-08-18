using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

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

    HurtByFire fireHurtTrigger;

    bool hasPath = false;
    bool safePath = false;

    public bool givingCPR = false;

    private void Start()
    {
        fireHurtTrigger = transform.Find("HurtByFire").GetComponent<HurtByFire>();
        anim = GetComponent<Animator>();

        hoverShape = transform.Find("HoverShape").gameObject;
        selectShape = transform.Find("SelectShape").gameObject;
        agent = GetComponent<NavMeshAgent>();

        currentDestination = transform.position;
    }

    private void Update()
    {
        if (givingCPR)
            return;

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

        if(hasPath && Vector3.Distance(currentDestination, transform.position) < 0.1f)
        {
            currentDestination = transform.position;
            agent.ResetPath();

            anim.SetBool("Running", false);

            hasPath = false;
            safePath = false;
        }

        if(hasPath && !safePath && fireHurtTrigger.nearFire > 0)
        {
            agent.ResetPath();
            hasPath = false;
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
        safePath = false;
        if (fireHurtTrigger.nearFire > 0)
        {
            int mask = LayerMask.GetMask("Flammable");
            bool hit = Physics.CheckSphere(point, 1f, mask, QueryTriggerInteraction.Collide);
            
            if (hit)
            {
                return;
            }
            else
            {
                anim.SetBool("NearFire", false);
                safePath = true;
            }
        }

        currentDestination = point;
        agent.SetDestination(point);
        hasPath = true;
        anim.SetBool("Running", true);
    }

    public void PerformCPR()
    {
        givingCPR = true;
        agent.ResetPath();
        hasPath = false;
        anim.SetTrigger("CPR");
    }
}
