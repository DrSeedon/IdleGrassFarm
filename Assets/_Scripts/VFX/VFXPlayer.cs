using UnityEngine;
using System.Collections.Generic;

public class VFXPlayer : MonoBehaviour
{
    public VFXData[] effects;

    Dictionary<VFXType, VFXData> vfxDict = new Dictionary<VFXType, VFXData>();
    Dictionary<VFXType, ParticleSystem> loopingVFX = new Dictionary<VFXType, ParticleSystem>();

    void Awake()
    {
        foreach (var vfx in effects)
        {
            vfxDict[vfx.type] = vfx;
        }
    }

    public void PlayVFX(VFXType type, Vector3 position, Quaternion rotation)
    {
        if (!vfxDict.TryGetValue(type, out VFXData data) || data.prefab == null)
        {
            return;
        }

        GameObject vfxObj = Instantiate(data.prefab, position, rotation);

        ParticleSystem ps = vfxObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            
            if (!ps.main.loop)
            {
                Destroy(vfxObj, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
        else
        {
            Destroy(vfxObj, 2f);
        }
    }

    public void PlayVFX(VFXType type, Vector3 position)
    {
        PlayVFX(type, position, Quaternion.identity);
    }

    public ParticleSystem StartLoopVFX(VFXType type, Transform parent)
    {
        if (loopingVFX.ContainsKey(type) && loopingVFX[type] != null)
        {
            return loopingVFX[type];
        }

        if (!vfxDict.TryGetValue(type, out VFXData data) || data.prefab == null)
        {
            return null;
        }

        GameObject vfxObj = Instantiate(data.prefab, parent);
        vfxObj.transform.localPosition = Vector3.zero;
        vfxObj.transform.localRotation = Quaternion.identity;

        ParticleSystem ps = vfxObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.loop = true;
            ps.Stop();
            loopingVFX[type] = ps;
            return ps;
        }

        Destroy(vfxObj);
        return null;
    }

    public void StopLoopVFX(VFXType type)
    {
        if (loopingVFX.ContainsKey(type) && loopingVFX[type] != null)
        {
            loopingVFX[type].Stop();
        }
    }

    public void PlayLoopVFX(VFXType type)
    {
        if (loopingVFX.ContainsKey(type) && loopingVFX[type] != null)
        {
            loopingVFX[type].Play();
        }
    }
}
