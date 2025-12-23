using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour
{
    GrassCutter cutter;
    SalesTable salesTable;
    UpgradeZone[] upgradeZones;
    AudioPlayer audioPlayer;

    [Inject]
    public void Construct(GrassCutter grassCutter, SalesTable sales, AudioPlayer audio)
    {
        cutter = grassCutter;
        salesTable = sales;
        audioPlayer = audio;
    }

    void Start()
    {
        upgradeZones = FindObjectsOfType<UpgradeZone>();

        if (cutter != null)
        {
            cutter.onMowerStarted.AddListener(OnMowerStarted);
            cutter.onMowerStopped.AddListener(OnMowerStopped);
        }

        if (salesTable != null)
        {
            salesTable.onGrassSold.AddListener(OnGrassSold);
        }

        foreach (var zone in upgradeZones)
        {
            zone.onUpgrade.AddListener(OnUpgrade);
        }
    }

    void OnDestroy()
    {
        if (cutter != null)
        {
            cutter.onMowerStarted.RemoveListener(OnMowerStarted);
            cutter.onMowerStopped.RemoveListener(OnMowerStopped);
        }

        if (salesTable != null)
        {
            salesTable.onGrassSold.RemoveListener(OnGrassSold);
        }

        foreach (var zone in upgradeZones)
        {
            if (zone != null)
            {
                zone.onUpgrade.RemoveListener(OnUpgrade);
            }
        }
    }

    void OnMowerStarted()
    {
        if (audioPlayer != null)
        {
            audioPlayer.PlayLoopSound(SoundType.MowerLoop);
        }
    }

    void OnMowerStopped()
    {
        if (audioPlayer != null)
        {
            audioPlayer.StopLoopSound(SoundType.MowerLoop, 0.1f);
        }
    }

    void OnGrassSold()
    {
        if (audioPlayer != null)
        {
            audioPlayer.PlaySound(SoundType.GrassSold);
        }
    }

    void OnUpgrade()
    {
        if (audioPlayer != null)
        {
            audioPlayer.PlaySound(SoundType.Upgrade);
        }
    }
}
