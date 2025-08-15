using UnityEngine;

public class EnterTruck : MonoBehaviour
{
    public GameObject interiorCamera;
    public GameObject firetruckInteriorOverlay;
    
    public FPSController playerController;

    SimpleCarController firetruckController;
    bool in_range = false;
    bool inside = false;
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
                Debug.Log($"Before SetActive: name={playerController.gameObject.name}, activeSelf={playerController.gameObject.activeSelf}, activeInHierarchy={playerController.gameObject.activeInHierarchy}");


                firetruckController.enabled = true;

                Invoke(nameof(DisablePlayer), 0.15f);

                Debug.Log($"After SetActive: name={playerController.gameObject.name}, activeSelf={playerController.gameObject.activeSelf}, activeInHierarchy={playerController.gameObject.activeInHierarchy}");

                interiorCamera.SetActive(true);
                firetruckInteriorOverlay.SetActive(true);

                Debug.Log("Got inside firetruck");
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
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        in_range = true;
    }

    private void OnTriggerExit(Collider other)
    {
        in_range = false;
    }


}
