using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ShootAFFF : MonoBehaviour
{
    ParticleSystem ps;
    bool sprayIsOn = false;

    AFFFPlane[] allFoamPlanes;
    private void Start()
    {
        allFoamPlanes = FindObjectsByType<AFFFPlane>(FindObjectsSortMode.None);
        ps = GetComponent<ParticleSystem>();
    }

    bool first = false;
    void Update()
    {
        bool mouseHeld = Input.GetMouseButton(0);

        if (mouseHeld)
        {
            sprayIsOn = true;
            ps.Play();
        }
        else if(!mouseHeld && !ps.isStopped)
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
    

            RaycastHit[] hits = Physics.RaycastAll(transform.parent.position, transform.parent.forward, 6.0f, layerMask: foamLayer);


            Debug.DrawLine(transform.position, transform.position + (transform.parent.forward * 5.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f));
            if(hits != null && hits.Length > 0)
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
