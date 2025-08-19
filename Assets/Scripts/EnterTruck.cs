using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnterTruck : MonoBehaviour
{
    public UnityEvent onEnterTruck;
    public UnityEvent onExitTruck;

    public GameObject interiorCamera;
    public GameObject firetruckInteriorOverlay;
    
    public FPSController playerController;

    SimpleCarController firetruckController;
    bool in_range = false;
    bool inside = false;

    public GameObject EButtonIcon;
    public GameObject spaceButton;

    void Start()
    {
        firetruckController = transform.parent.GetComponent<SimpleCarController>();
    }

    void DisablePlayer()
    {
        playerController.gameObject.SetActive(false);
    }

    void Update()
    {
        if(!inside)
        {
            if (in_range && Input.GetKeyDown(KeyCode.E))
            {
                inside = true;
                playerController.gameObject.transform.parent = transform;

                firetruckController.enabled = true;

                DisablePlayer();

                interiorCamera.SetActive(true);
                firetruckInteriorOverlay.SetActive(true);

                EButtonIcon.SetActive(true);
                EButtonIcon.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Exit Truck";

                spaceButton.SetActive(true);
                spaceButton.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Siren";

                onEnterTruck?.Invoke();
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                inside = false;
                playerController.gameObject.transform.parent = null;
                firetruckController.enabled = false;
                playerController.gameObject.SetActive(true);

                interiorCamera.SetActive(false);
                firetruckInteriorOverlay.SetActive(false);

                Debug.Log("Got out firetruck");

                spaceButton.SetActive(false);
                EButtonIcon.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Enter Truck";

                onExitTruck?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EButtonIcon.SetActive(true);
        EButtonIcon.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Enter Truck";

        in_range = true;
    }

    private void OnTriggerExit(Collider other)
    {
        EButtonIcon.SetActive(false);
        in_range = false;
    }


}
