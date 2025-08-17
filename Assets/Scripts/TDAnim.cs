using UnityEngine;

public class TDAnim : MonoBehaviour
{
    public bool getOut;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("GetOut", getOut);
        
    }

    public void GetOut()
    {
        anim.SetBool("GetOut", true);
        //PrimeTween.Tween.Custom()
    }
}
