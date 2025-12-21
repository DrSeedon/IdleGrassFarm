using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class Customer : MonoBehaviour
{
    public CustomerState state = CustomerState.Wandering;
    public CustomerOrder order;
    public GrassInventory inventory;

    NavMeshAgent agent;
    QueueManager assignedQueue;
    Transform currentQueuePoint;
    Transform exitPoint;

    float wanderTimer;
    float wanderInterval = 3f;
    float queueRetryTimer;
    float queueRetryInterval = 2f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        inventory = GetComponent<GrassInventory>();
        order = new CustomerOrder();
    }

    void Start()
    {
        order.GenerateRandomOrder();
        FindAndJoinQueue();
    }

    void Update()
    {
        switch (state)
        {
            case CustomerState.Wandering:
                UpdateWandering();
                break;
            case CustomerState.MovingToQueue:
                UpdateMovingToQueue();
                break;
            case CustomerState.InQueue:
                break;
            case CustomerState.Leaving:
                UpdateLeaving();
                break;
        }
    }

    void FindAndJoinQueue()
    {
        QueueManager[] allQueues = FindObjectsOfType<QueueManager>();
        
        Debug.Log($"Customer: Found {allQueues.Length} queues");
        
        if (allQueues.Length == 0)
        {
            Debug.LogWarning("Customer: No QueueManager found! Make sure QueueManager is on the scene.");
            state = CustomerState.Wandering;
            return;
        }

        QueueManager nearest = allQueues[0];
        float nearestDist = Vector3.Distance(transform.position, nearest.transform.position);

        foreach (var queue in allQueues)
        {
            float dist = Vector3.Distance(transform.position, queue.transform.position);
            if (dist < nearestDist)
            {
                nearest = queue;
                nearestDist = dist;
            }
        }

        assignedQueue = nearest;
        Debug.Log($"Customer: Assigned to queue at distance {nearestDist}");
        TryJoinQueue();
    }

    void TryJoinQueue()
    {
        if (assignedQueue == null)
        {
            Debug.LogWarning("Customer: assignedQueue is null!");
            return;
        }

        bool joined = assignedQueue.TryJoinQueue(this);
        Debug.Log($"Customer: TryJoinQueue result = {joined}");
        
        if (joined)
        {
            state = CustomerState.MovingToQueue;
        }
        else
        {
            state = CustomerState.Wandering;
        }
    }

    void UpdateWandering()
    {
        wanderTimer -= Time.deltaTime;
        queueRetryTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            WanderToRandomPoint();
            wanderTimer = Random.Range(wanderInterval, wanderInterval + 2f);
        }

        if (queueRetryTimer <= 0f)
        {
            TryJoinQueue();
            queueRetryTimer = queueRetryInterval;
        }
    }

    void WanderToRandomPoint()
    {
        if (agent == null) return;

        Vector3 randomDir = Random.insideUnitSphere * 10f;
        randomDir += transform.position;
        randomDir.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void UpdateMovingToQueue()
    {
        if (agent == null || currentQueuePoint == null)
        {
            state = CustomerState.Wandering;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = CustomerState.InQueue;
        }
    }

    void UpdateLeaving()
    {
        if (agent == null || exitPoint == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetQueuePoint(Transform point)
    {
        currentQueuePoint = point;
        if (agent != null && point != null)
        {
            agent.SetDestination(point.position);
        }
    }

    public void StartLeaving(Transform exit)
    {
        state = CustomerState.Leaving;
        exitPoint = exit;
        
        if (assignedQueue != null)
        {
            assignedQueue.RemoveCustomer(this);
        }

        if (agent != null && exit != null)
        {
            agent.SetDestination(exit.position);
        }
    }

    public void ReceiveGrass(GrassType type, Color color, int count)
    {
        if (inventory != null)
        {
            inventory.AddGrass(type, color, count);
        }
    }

    void OnGUI()
    {
        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        
        if (screenPos.z > 0)
        {
            screenPos.y = Screen.height - screenPos.y;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 12;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            string stateText = $"State: {state}";
            string orderText = "Order: ";
            
            if (order != null && order.requirements != null)
            {
                foreach (var req in order.requirements)
                {
                    orderText += $"{req.Key}x{req.Value} ";
                }
            }

            string fullText = stateText + "\n" + orderText;
            
            Vector2 size = style.CalcSize(new GUIContent(fullText));
            GUI.Box(new Rect(screenPos.x - size.x * 0.5f, screenPos.y - size.y * 0.5f, size.x, size.y), fullText, style);
        }
    }
}
