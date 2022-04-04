using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        audio.Play();
    }
}
