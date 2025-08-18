using UnityEngine;

public class AFFFStorage : MonoBehaviour
{
    public GameObject playerAFFF;
    bool playerNearby = false;

    private void Update()
    {
        if (playerNearby)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                playerAFFF.SetActive(!playerAFFF.activeSelf);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerNearby = false;
    }
}
