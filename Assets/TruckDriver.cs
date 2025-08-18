using UnityEngine;

public class TruckDriver : MonoBehaviour
{

    public bool readyForMedic;
    bool nearPlayer;
    Medic nearMedic;

    void Start()
    {
        
    }

    void Update()
    {
        if(!readyForMedic && nearPlayer)
        {

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
            nearPlayer = false;
        }
    }
}
