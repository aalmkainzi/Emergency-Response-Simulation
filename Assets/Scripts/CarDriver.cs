using System.Collections;
using UnityEngine;

public class CarDriver : MonoBehaviour
{

    public bool readyForMedic;
    bool nearPlayer;
    Medic nearMedic;

    [SerializeField] Vector3 newPos;
    Animator anim;

    public bool gettingCPR;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!readyForMedic && nearPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                readyForMedic = true;
                transform.localPosition = newPos;
                anim.SetBool("Lay", true);
                readyForMedic = true;
            }
        }

        if (readyForMedic && nearMedic)
        {
            nearMedic.PerformCPR();
            readyForMedic = false;
            gettingCPR = true;

            StartCoroutine(CheckIfCPRDone(nearMedic));
        }
    }

    IEnumerator CheckIfCPRDone(Medic medic)
    {
        while (true)
        {
            if (!medic.givingCPR)
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Medic"))
        {
            nearMedic = other.gameObject.GetComponent<Medic>();
        }
        else // player
        {
            nearPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Medic"))
        {
            if (nearMedic == other.gameObject.GetComponent<Medic>())
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
            nearPlayer = false;
        }
    }
}
