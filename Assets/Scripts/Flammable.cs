using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public int resistance;
    public bool onFire;

    public List<Flammable> nearbyFlammables;

    private void Start()
    {
        nearbyFlammables = new();
        InvokeRepeating(nameof(EnflameNearby), 1.0f, 1.0f);
        
        if(onFire)
        {
            ParticleSystem ps = transform.Find("FirePS").GetComponent<ParticleSystem>();
            ps.Play(true);
        }
    }

    public void SetOnFire()
    {
        onFire = true;
        ParticleSystem ps = transform.Find("FirePS").GetComponent<ParticleSystem>();
        ps.Play(true);
    }

    public void Enflame()
    {
        if(onFire)
            return;

        resistance -= 1;
        if(resistance <= 0)
        {
            SetOnFire();
        }
    }

    void EnflameNearby()
    {
        if(onFire)
        {
            foreach(Flammable f in nearbyFlammables)
            {
                f.Enflame();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(gameObject.name + " triggered by " + other.gameObject.name);
        Flammable nearbyFlammable = other.gameObject.GetComponent<Flammable>();
        if(nearbyFlammable != null)
            nearbyFlammables.Add(nearbyFlammable);
    }
}
