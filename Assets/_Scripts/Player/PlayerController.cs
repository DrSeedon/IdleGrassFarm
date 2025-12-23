using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Transform modelTransform;
    public Animator animator;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            Vector3 targetPosition = transform.position + direction * 10f;
            
            if (agent != null)
            {
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
            }

            if (modelTransform != null && agent != null && agent.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
        else
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
        }

        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("Speed", speed);
        }
    }
}

