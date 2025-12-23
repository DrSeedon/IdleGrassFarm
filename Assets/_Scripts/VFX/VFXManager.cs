using UnityEngine;
using Zenject;

public class VFXManager : MonoBehaviour
{
    SalesTable salesTable;
    UpgradeZone[] upgradeZones;
    VFXPlayer vfxPlayer;

    [Inject]
    public void Construct(SalesTable sales, VFXPlayer vfx)
    {
        salesTable = sales;
        vfxPlayer = vfx;
    }

    void Start()
    {
        upgradeZones = FindObjectsOfType<UpgradeZone>();

        if (salesTable != null)
        {
            salesTable.onGrassSold.AddListener(OnGrassSold);
        }

        foreach (var zone in upgradeZones)
        {
            zone.onUpgrade.AddListener(() => OnUpgrade(zone.transform.position));
        }
    }

    void OnDestroy()
    {
        if (salesTable != null)
        {
            salesTable.onGrassSold.RemoveListener(OnGrassSold);
        }
    }

    void OnGrassSold()
    {
        if (vfxPlayer != null && salesTable != null)
        {
            vfxPlayer.PlayVFX(VFXType.GrassSold, salesTable.transform.position + Vector3.up * 1.5f);
        }
    }

    void OnUpgrade(Vector3 position)
    {
        if (vfxPlayer != null)
        {
            vfxPlayer.PlayVFX(VFXType.Upgrade, position + Vector3.up * 1f);
        }
    }
}
