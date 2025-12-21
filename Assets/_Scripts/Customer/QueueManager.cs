using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    public List<Transform> queuePoints = new List<Transform>();
    
    List<Customer> queue = new List<Customer>();

    public int MaxQueueSize => queuePoints.Count;

    public bool TryJoinQueue(Customer customer)
    {
        if (queuePoints.Count == 0)
        {
            Debug.LogError("QueueManager: No queue points assigned! Add child transforms to queuePoints list.");
            return false;
        }

        if (queue.Count >= MaxQueueSize)
        {
            Debug.Log($"QueueManager: Queue is full ({queue.Count}/{MaxQueueSize})");
            return false;
        }

        queue.Add(customer);
        int index = queue.Count - 1;
        Debug.Log($"QueueManager: Customer joined at position {index}");
        AssignQueuePoint(customer, index);
        return true;
    }

    void AssignQueuePoint(Customer customer, int index)
    {
        if (index < 0 || index >= queuePoints.Count) return;

        customer.SetQueuePoint(queuePoints[index]);
    }

    public Customer GetFirstCustomer()
    {
        if (queue.Count == 0) return null;
        return queue[0];
    }

    public void RemoveCustomer(Customer customer)
    {
        queue.Remove(customer);
    }

    public void ShiftQueue()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            AssignQueuePoint(queue[i], i);
        }
    }

    public int GetQueuePosition(Customer customer)
    {
        return queue.IndexOf(customer);
    }

    void OnDrawGizmosSelected()
    {
        if (queuePoints == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < queuePoints.Count; i++)
        {
            if (queuePoints[i] == null) continue;

            Gizmos.DrawWireSphere(queuePoints[i].position, 0.3f);
            
            if (i > 0 && queuePoints[i - 1] != null)
            {
                Gizmos.DrawLine(queuePoints[i - 1].position, queuePoints[i].position);
            }
        }
    }
}
