using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TruckDriver : MonoBehaviour
{

    public bool readyForMedic;
    bool nearPlayer;
    Medic nearMedic;

    [SerializeField] Vector3 newPos;
    Animator anim;

    public bool gettingCPR;

    public Transform toMove;

    public GameObject eButton;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!readyForMedic && nearPlayer && TaskManager.instance.currentTask == TaskManager.PlayerTask.RESCUE_DRIVERS)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                readyForMedic = true;
                toMove.localPosition = newPos;
                anim.SetBool("Lay", true);
                readyForMedic = true;
            }
        }

        if(readyForMedic && nearMedic)
        {
            nearMedic.PerformCPR();
            readyForMedic = false;
            gettingCPR = true;

            StartCoroutine(CheckIfCPRDone(nearMedic));
        }
    }

    IEnumerator CheckIfCPRDone(Medic medic)
    {
        while(true)
        {
            if(!medic.givingCPR)
            {
                toMove.gameObject.SetActive(false);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Medic"))
        {
            nearMedic = other.gameObject.GetComponent<Medic>();
        }
        else // player
        {
            nearPlayer = true;
            eButton.SetActive(true);
            eButton.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Help";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Medic"))
        {
            if(nearMedic == other.gameObject.GetComponent<Medic>())
            {
                nearMedic = null;
            }
            else
            {
                nearMedic = other.gameObject.GetComponent<Medic>();
            }
            
        }
        else // player
        {
            eButton.SetActive(false);
            nearPlayer = false;
        }
    }
}
