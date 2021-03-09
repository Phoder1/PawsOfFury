using UnityEngine;
using UnityEngine.AI;

public class NavScript : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float range;
    // Start is called before the first frame update
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    public void StartMove(Vector3? target = null)
    {
        if (target != null)
            agent.SetDestination((Vector3)target);
        agent.isStopped = false;
    }
    public void StopMove()
    {
        if (isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }
}
