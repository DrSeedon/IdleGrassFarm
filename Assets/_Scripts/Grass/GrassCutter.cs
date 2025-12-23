using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GrassCutter : MonoBehaviour
{
    public float cutRadius = 1f;
    public float cutInterval = 0.1f;
    public GrassInventory inventory;
    public Transform visualObject;
    public float rotationSpeed = 360f;
    public float stopSoundDelay = 0.3f;
    public ParticleSystem grassCutVFX;

    public UnityEvent onMowerStarted;
    public UnityEvent onMowerStopped;

    float cutTimer = 0f;
    float stopSoundTimer = 0f;
    bool isCutting = false;
    bool wasCutting = false;
    Dictionary<GrassType, int> cutCounts = new Dictionary<GrassType, int>();

    void Start()
    {
        if (grassCutVFX != null)
        {
            grassCutVFX.Stop();
            UpdateVFXScale();
        }
    }

    void Update()
    {
        HandleMowerSound();

        if (inventory != null && inventory.IsFull()) return;

        if (visualObject != null)
        {
            visualObject.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        cutTimer -= Time.deltaTime;

        if (cutTimer <= 0f)
        {
            CutAllGrass();
            cutTimer = cutInterval;
        }
    }

    void HandleMowerSound()
    {
        if (isCutting && !wasCutting)
        {
            onMowerStarted?.Invoke();
            if (grassCutVFX != null)
            {
                grassCutVFX.Play();
            }
            wasCutting = true;
            stopSoundTimer = 0f;
        }
        else if (!isCutting && wasCutting)
        {
            stopSoundTimer += Time.deltaTime;
            if (stopSoundTimer >= stopSoundDelay)
            {
                onMowerStopped?.Invoke();
                if (grassCutVFX != null)
                {
                    grassCutVFX.Stop();
                }
                wasCutting = false;
            }
        }
        else if (isCutting && wasCutting)
        {
            stopSoundTimer = 0f;
        }

        isCutting = false;
    }

    void CutAllGrass()
    {
        GrassFieldRenderer[] fields = FindObjectsOfType<GrassFieldRenderer>();
        
        bool anyCut = false;
        Color dominantColor = Color.green;
        int maxCutCount = 0;
        
        foreach (var field in fields)
        {
            int cutCount = field.CutGrassInRadius(transform.position, cutRadius);
            
            if (cutCount > 0)
            {
                GrassType type = field.GetGrassType();
                
                if (!cutCounts.ContainsKey(type))
                {
                    cutCounts[type] = 0;
                }
                
                cutCounts[type] += cutCount;
                Debug.Log($"Cut {type} grass: +{cutCount} (Total: {cutCounts[type]})");

                if (inventory != null && field.grassData != null)
                {
                    inventory.AddGrass(type, field.grassData.color, cutCount);
                    
                    if (cutCount > maxCutCount)
                    {
                        maxCutCount = cutCount;
                        dominantColor = field.grassData.color;
                    }
                }

                anyCut = true;
            }
        }

        if (anyCut)
        {
            isCutting = true;
            
            if (grassCutVFX != null)
            {
                ParticleSystemRenderer renderer = grassCutVFX.GetComponent<ParticleSystemRenderer>();
                if (renderer != null && renderer.material != null)
                {
                    Color darkerColor = dominantColor * 0.7f;
                    darkerColor.a = 0.6f;
                    renderer.material.SetColor("_BaseColor", darkerColor);
                    renderer.material.SetColor("_TopColor", dominantColor);
                }
            }
        }
    }

    public void UpgradeRadius(float amount)
    {
        cutRadius += amount;
        
        if (visualObject != null)
        {
            float newScale = cutRadius * 2f;
            visualObject.localScale = Vector3.one * newScale;
        }
        
        UpdateVFXScale();
        
        Debug.Log($"Cut radius upgraded to {cutRadius}");
    }

    void UpdateVFXScale()
    {
        if (grassCutVFX != null)
        {
            float vfxScale = cutRadius * 2f;
            grassCutVFX.transform.localScale = Vector3.one * vfxScale;
        }
    }

    public int GetCutCount(GrassType type)
    {
        return cutCounts.ContainsKey(type) ? cutCounts[type] : 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cutRadius);
    }
}

