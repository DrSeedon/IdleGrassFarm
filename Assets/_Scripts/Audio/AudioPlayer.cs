using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AudioPlayer : MonoBehaviour
{
    public SoundData[] sounds;

    Dictionary<SoundType, SoundData> soundDict = new Dictionary<SoundType, SoundData>();
    Dictionary<SoundType, AudioSource> loopingSources = new Dictionary<SoundType, AudioSource>();

    void Awake()
    {
        foreach (var sound in sounds)
        {
            soundDict[sound.type] = sound;
        }
    }

    public void PlaySound(SoundType type)
    {
        if (!soundDict.TryGetValue(type, out SoundData data) || data.clip == null)
        {
            return;
        }

        GameObject soundObj = new GameObject($"Sound_{type}");
        soundObj.transform.SetParent(transform);

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.Play();

        CleanupAfterPlay(soundObj, data.clip.length).Forget();
    }

    public void PlayLoopSound(SoundType type)
    {
        if (loopingSources.ContainsKey(type) && loopingSources[type] != null)
        {
            return;
        }

        if (!soundDict.TryGetValue(type, out SoundData data) || data.clip == null)
        {
            return;
        }

        GameObject soundObj = new GameObject($"Loop_{type}");
        soundObj.transform.SetParent(transform);

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = true;
        source.Play();

        loopingSources[type] = source;
    }

    public void StopLoopSound(SoundType type, float fadeOutTime = 0f)
    {
        if (!loopingSources.ContainsKey(type) || loopingSources[type] == null)
        {
            return;
        }

        AudioSource source = loopingSources[type];
        loopingSources.Remove(type);

        if (fadeOutTime > 0f)
        {
            FadeOutAndStop(source, fadeOutTime).Forget();
        }
        else
        {
            Destroy(source.gameObject);
        }
    }

    async UniTaskVoid FadeOutAndStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration && source != null)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            await UniTask.Yield();
        }

        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }

    async UniTaskVoid CleanupAfterPlay(GameObject obj, float duration)
    {
        await UniTask.Delay((int)(duration * 1000));
        
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}
