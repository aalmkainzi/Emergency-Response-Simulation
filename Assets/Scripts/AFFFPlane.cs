using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFFFPlane : MonoBehaviour
{
    public float foamLevel = 0;
    float maxFoam = 1.0f;
    Material mat;
    public bool applyingFoam;
    

    public List<Flammable> nearbyFlammables;

    bool done = false;
    bool fullyFoamed = false;
    [SerializeField] float maxHilLHeight = 2;
    void Start()
    {
        nearbyFlammables = new();
        mat = GetComponent<MeshRenderer>().material;
        StartCoroutine(ApplyFoam());
    }

    IEnumerator ApplyFoam()
    {
        WaitForSeconds tickWait = new WaitForSeconds(0.1f);
        PrimeTween.Tween tween = new PrimeTween.Tween();
        while(true)
        {
            if(applyingFoam)
            {
                float hillHeight = mat.GetFloat("_HillHeight");
                if (hillHeight >= maxHilLHeight)
                {
                    fullyFoamed = true;
                    yield break;
                }
                foamLevel += 0.1f;
                tween.Stop();
                tween = PrimeTween.Tween.Custom(hillHeight, hillHeight + foamLevel, 0.1f,
                onValueChange: (float newVal) =>
                {
                    mat.SetFloat("_HillHeight", newVal);
                    mat.SetFloat("_Radius", newVal / 4);
                });
            }

            yield return tickWait;
        }
    }

    void Update()
    {
        if(fullyFoamed && !done)
        {
            Debug.Log("About to disable " + nearbyFlammables.Count + " flamables");
            foreach(Flammable f in nearbyFlammables)
            {
                f.gameObject.SetActive(false);
            }
            done = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Flammable f = other.gameObject.GetComponent<Flammable>();
        if(f != null)
        {
            nearbyFlammables.Add(f);
        }
    }

}
