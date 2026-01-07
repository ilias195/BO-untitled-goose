using UnityEngine;
using UnityEngine.AI;

public class FarmerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent _agent;

    void Update()
    {
        if (_agent != null && animator != null)
        {
           
            float speed = _agent.velocity.magnitude;
            animator.SetBool("Speed", true);
        }
        else
        {
            animator.SetBool("Speed",false);
        }
    }
}
