using UnityEngine;

[System.Serializable]
public class SoundData
{
    public SoundType type;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
}
