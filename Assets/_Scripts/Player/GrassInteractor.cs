using UnityEngine;
using Zenject;

public class GrassInteractor : MonoBehaviour
{
    public float radius = 1.5f;
    public float heightOffset = 0f;

    GrassManager grassManager;

    [Inject]
    public void Construct(GrassManager manager)
    {
        grassManager = manager;
    }

    void Update()
    {
        if (grassManager == null) return;

        Vector3 pos = transform.position;
        pos.y += heightOffset;
        
        grassManager.SetTramplePosition(pos, radius);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
