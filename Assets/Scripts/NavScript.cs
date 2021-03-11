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
        agent.SetDestination(LevelManager._instance.levelEndPos);
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
