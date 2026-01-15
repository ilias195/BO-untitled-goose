using UnityEngine;

public class FarmerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // lopen
    public void PlayWalk()
    {
        animator.SetBool("IsWalking", true);
    }

    public void StopWalk()
    {
        animator.SetBool("IsWalking", false);
    }

    // taak animaties
    public void PlayTaskAnimation(FarmerTask task)
    {
        Reset();

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsWorking", true);

        if (task == FarmerTask.Watering)
         animator.SetBool("IsWatering", true);

        else if (task == FarmerTask.Rake)
         animator.SetBool("IsRaking", true);
    }

    public void StopTaskAnimation()
    {
        Reset();
        animator.SetBool("IsWorking", false);
    }

    void Reset()
    {
        animator.SetBool("IsWatering", false);
        animator.SetBool("IsRaking", false);
    }
}
