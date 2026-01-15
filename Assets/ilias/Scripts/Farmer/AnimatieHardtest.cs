using UnityEngine;

public class AnimatieHardtest : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        Debug.Log("Animator found: " + anim);

        anim.Play(0); // forceer eerste state
    }
}
