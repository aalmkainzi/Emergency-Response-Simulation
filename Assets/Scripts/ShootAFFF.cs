using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ShootAFFF : MonoBehaviour
{
    ParticleSystem ps;
    bool sprayIsOn = false;
    bool psOn = false;

    AFFFPlane[] allFoamPlanes;
    private void Start()
    {
        allFoamPlanes = FindObjectsByType<AFFFPlane>(FindObjectsSortMode.None);
        ps = GetComponent<ParticleSystem>();
    }

    bool first = false;
    void Update()
    {
        bool spaceHeld = Input.GetKey(KeyCode.Space);

        if (spaceHeld && !sprayIsOn)
        {
            sprayIsOn = true;
            ps.Play();
        }
        else if(!spaceHeld && sprayIsOn)
        {
            sprayIsOn = false;
            ps.Stop();
        }

        foreach (AFFFPlane a in allFoamPlanes)
        {
            a.applyingFoam = false;
        }
        if(sprayIsOn)
        {
            int foamLayer = 1 << LayerMask.NameToLayer("Foam");
            if (!first) { Debug.Log("foamLayer = " + foamLayer); first = true;  }

            // RaycastHit[] hits = Physics.RaycastAll(transform.parent.position, transform.parent.forward, 7.5f, layerMask: foamLayer);
            RaycastHit[] hits = Physics.CapsuleCastAll(transform.parent.position, transform.parent.position + (transform.parent.forward * 7.5f), 3.0f, transform.parent.forward, 1.0f, layerMask: foamLayer);
            
            if (hits != null && hits.Length > 0)
            {

                foreach(RaycastHit rch in hits)
                {
                    AFFFPlane foamBlock = rch.collider.gameObject.GetComponent<AFFFPlane>();
                    foamBlock.applyingFoam = true;
                }
            }
        }
    }
}
