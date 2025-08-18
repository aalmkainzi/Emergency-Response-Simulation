using UnityEngine;
using UnityEngine.Events;

public class HurtByFire : MonoBehaviour
{
    public int nearFire;
    public UnityEvent onNearFire;

    [SerializeField] Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " Collided with fire");
        Flammable otherFlammable;
        bool exist = other.gameObject.TryGetComponent<Flammable>(out otherFlammable);

        if(exist && otherFlammable.onFire)
        {
            Debug.Log(gameObject.name + " Enabled fire reaction");
            if(nearFire == 0) anim.SetBool("NearFire", true);
            nearFire += 1;

            onNearFire?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nearFire -= 1;
    }
}
