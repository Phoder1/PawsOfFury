using UnityEngine;
using UnityEngine.AI;

public class NavScript : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] float range;
    Unit unit;
    // Start is called before the first frame update
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SetDestination(BlackBoard.levelManager.LevelEndPos);
    }
    public void SetDestination(Vector3 destination)
    {
        if (agent.destination != destination)
            agent.SetDestination(destination);
    }
    public void StartMove()
    {
        agent.isStopped = false;
    }
    public void StopMove()
    {
        if (isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }
}
