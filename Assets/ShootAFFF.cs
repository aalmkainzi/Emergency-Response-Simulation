using UnityEngine;

public class ShootAFFF : MonoBehaviour
{
    ParticleSystem ps;
    bool sprayIsOn = false;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

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
    }
}
