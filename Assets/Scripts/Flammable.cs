using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flammable : MonoBehaviour
{
    public int resistance;
    public bool onFire;

    public List<Flammable> nearbyFlammables;

    public static int nbOnFire = 0;
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
        if (onFire || !gameObject.activeSelf) return;

        onFire = true;
        nbOnFire += 1;
        Debug.Log("Flammable: nbOnFire = " + nbOnFire);
        Transform child = transform.Find("FirePS");

        if (child == null)
        {
            Debug.Log(gameObject.name + " does not have a FirePS child");
            Debug.Log("List of children");

            string listOfChildren = "";

            foreach (Transform t in transform)
            {
                listOfChildren += t.name + ", ";
            }

            Debug.Log(listOfChildren);
            
        }

        ParticleSystem ps = child.GetComponent<ParticleSystem>();


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
