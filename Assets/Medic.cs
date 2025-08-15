using UnityEngine;

public class Medic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Flammable flammable;
        bool found = other.gameObject.TryGetComponent(out flammable);
        if (found)
        {
            if (flammable.onFire)
            {
                
            }
        }
    }
}
