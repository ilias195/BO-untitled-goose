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

        animator.SetBool("IsRaking", task == FarmerTask.Rake);
        animator.SetBool("IsDigging", task == FarmerTask.Digging);
    }

    public void StopTaskAnimation()
    {
        animator.SetBool("IsWorking", false);
        animator.SetBool("IsRaking", false);
        animator.SetBool("IsDigging", false); 
    }
}
