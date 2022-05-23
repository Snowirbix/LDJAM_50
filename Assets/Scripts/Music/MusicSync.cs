using UnityEngine;
using UnityEngine.VFX;

public class MusicSync : MonoBehaviour
{
    public int bpm;
    [ReadOnly]
    public double secondPerBeat;
    [ReadOnly]
    public double songTime;
    [ReadOnly]
    public int songBeat;
    [ReadOnly]
    public double dspStartTime;

    public VisualEffect vfx;
    private AudioSource source;

    private void Start()
    {
        source = this.Q<AudioSource>();
        secondPerBeat = 60d / bpm;
        dspStartTime = AudioSettings.dspTime;
        source.Play();
    }

    private void Update()
    {
        songTime = AudioSettings.dspTime - dspStartTime;
        var newSongBeat = Mathf.FloorToInt((float)(songTime / secondPerBeat));
        if (newSongBeat > songBeat)
        {
            vfx.Play();
        }
        songBeat = newSongBeat;
    }
}
