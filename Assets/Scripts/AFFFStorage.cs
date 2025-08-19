using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AFFFStorage : MonoBehaviour
{
    public GameObject playerAFFF;
    bool playerNearby = false;

    public GameObject spaceIcon;
    public GameObject eButton;
    private void Update()
    {
        if (playerNearby)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(playerAFFF.activeSelf)
                {
                    playerAFFF.SetActive(false);
                    spaceIcon.SetActive(false);
                }
                else
                {
                    playerAFFF.SetActive(true);
                    spaceIcon.SetActive(true);
                    spaceIcon.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Spray";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        eButton.SetActive(true);
        eButton.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = "Take Foam";
        playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        eButton.SetActive(false);
        playerNearby = false;
    }
}
