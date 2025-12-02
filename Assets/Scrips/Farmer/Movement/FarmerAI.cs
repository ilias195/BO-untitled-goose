using UnityEngine;
using UnityEngine.AI;

public class FarmerAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    public float walkRadius = 6f;
    public float walkDelay = 3f;

    void Start()
    {
        // Begin meteen met lopen naar een klein stukje voorwaarts
        Vector3 firstTarget = transform.position + transform.forward * 2f;
        _agent.SetDestination(firstTarget);

        // Start willekeurig rondwandelen
        InvokeRepeating(nameof(WalkRandom), walkRadius, walkDelay);
    }

    void WalkRandom()
    {
        // Kies een willekeurige positie binnen de wanderRadius
        Vector3 randomPos = transform.position + Random.insideUnitSphere * walkRadius;

        // Zorg dat positie op de NavMesh ligt
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, walkRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
}
