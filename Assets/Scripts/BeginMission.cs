using UnityEngine;
using UnityEngine.Events;

public class BeginMission : MonoBehaviour
{
    public AudioSource fireSound;
    public Flammable[] startingFire;
    public UnityEvent onEnter;

    public static BeginMission instance;

    private void Start()
    {
        if(instance != null)
        {
            Destroy(instance);
        }

        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        onEnter?.Invoke();
        fireSound.Play();
        foreach (var flammable in startingFire)
        {
            flammable.SetOnFire();
        }

        Debug.Log("Player entered mission area");
    }
}
