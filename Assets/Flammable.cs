using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    public int resistance;
    public bool onFire;

    List<Flammable> inRange;

    private void Start()
    {
        inRange = new();
        InvokeRepeating(nameof(EnflameNearby), 1.0f, 1.0f);
        
        if(onFire)
        {
            ParticleSystem ps = transform.Find("FirePS").GetComponent<ParticleSystem>();
            ps.Play(true);
        }
    }

    public void Enflame()
    {
        if(onFire) return;

        resistance -= 1;
        if(resistance <= 0)
        {
            onFire = true;
            ParticleSystem ps = transform.Find("FirePS").GetComponent<ParticleSystem>();
            ps.Play(true);
        }
    }

    void EnflameNearby()
    {
        if(onFire)
        {
            foreach(Flammable f in inRange)
            {
                f.Enflame();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Flammable nearbyFlammable = other.gameObject.GetComponent<Flammable>();
        inRange.Add(nearbyFlammable);
    }
}
