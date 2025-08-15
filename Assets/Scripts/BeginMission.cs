using UnityEngine;

public class BeginMission : MonoBehaviour
{
    public Flammable[] startingFire;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var flammable in startingFire)
        {
            flammable.SetOnFire();
        }

        Debug.Log("Player entered mission area");
    }
}
