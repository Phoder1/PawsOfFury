using UnityEngine;
using UnityEngine.AI;

public class NavScript : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] float range;
    Unit unit;
    float speed;
    public float Speed
    {
        get => speed;
        set
        {
            if(speed != value)
            {
                speed = value;
                agent.speed = Speed;
            }
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    public void StartMove(Vector3? destination = null)
    {
        agent.enabled = true;
        if (destination != null)
            agent.destination = (Vector3)destination;
        agent.isStopped = false;
    }
    public void StopMove()
    {
        if (isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            //agent.enabled = false;
        }
    }
}
