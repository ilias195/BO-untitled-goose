using UnityEngine;

public class GooseAnimations : MonoBehaviour
{
    [SerializeField] private static Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();

        _animator.SetBool("Idle", true);

        _animator.SetBool("Walking", false);

        _animator.SetBool("Running", false);

        _animator.SetBool("Crouching", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlayIdleAnimation()
    {
        _animator.SetBool("Idle", true);

        _animator.SetBool("Walking", false);

        _animator.SetBool("Running", false);

        _animator.SetBool("Crouching", false);
    }

    public static void PlayWalkingAnimation()
    {
        _animator.SetBool("Walking", true);

        _animator.SetBool("Idle", false);

        _animator.SetBool("Running", false);

        _animator.SetBool("Crouching", false);
    }

    public static void PlayRunAnimation()
    {
        _animator.SetBool("Running", true);

        _animator.SetBool("Idle", false);

        _animator.SetBool("Walking", false);

        _animator.SetBool("Crouching", false);
    }

    public static void PlayCrouchingAnimation()
    {
        _animator.SetBool("Crouching", true);

        _animator.SetBool("Idle", false);

        _animator.SetBool("Walking", false);

        _animator.SetBool("Running", false);
    }
}
