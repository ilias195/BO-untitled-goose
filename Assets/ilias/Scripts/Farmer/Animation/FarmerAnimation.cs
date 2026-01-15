using UnityEngine;

public class FarmerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool moving)
    {
        animator.SetBool("IsMoving", moving);
    }

    public void PlayTaskAnimation(FarmerTask task)
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsWorking", true);

        animator.SetBool("IsWatering", task == FarmerTask.Watering);
        animator.SetBool("IsRaking", task == FarmerTask.Rake);
    }

    public void StopTaskAnimation()
    {
        animator.SetBool("IsWorking", false);
        animator.SetBool("IsWatering", false);
        animator.SetBool("IsRaking", false);
    }
}
