using UnityEngine;
using MidiSharp;
using System.IO;
using MidiSharp.Events;
using MidiSharp.Events.Voice.Note;
using MidiSharp.Events.Meta;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Collections;
using System;
using System.Linq;

public class MidiController : MonoBehaviour
{
    [Serializable]
    public record Note
    {
        public float Time { get; set; }
        public float Velocity { get; set; }
    }
    [ReadOnly]
    public Queue<Note> notes = new();
    [ReadOnly]
    public int ticksPerBeat;

    public float bpm = 160f;
    public float bps;
    public float tickPerSecond;

    public VisualEffect vfx;

    private float startTime;
    private float startMusicTime;

    public float timeSpent;
    public float musicTimeSpent;

    private IEnumerator coroutine;

    private int intensityId;

    private List<Animator> animators = new ();
    private AudioSource audio;

    public string midiFile = "Music/drum_midi";

    private void Start()
    {
        bps = bpm / 60f;
        animators = GetComponentsInChildren<Animator>().ToList();
        audio = this.Q<AudioSource>();
        ParseMidi(midiFile);
        tickPerSecond = bps * ticksPerBeat;
        audio.Play();
        startTime = Time.time;
        startMusicTime = (float)AudioSettings.dspTime;
        coroutine = CheckTheBeat();
        StartCoroutine(coroutine);
        intensityId = Shader.PropertyToID("Intensity");
    }

    IEnumerator CheckTheBeat()
    {
        yield return null;
        while (true)
        {
            var deltaTime = Time.time - startTime;
            timeSpent = deltaTime;
            musicTimeSpent = (float)AudioSettings.dspTime - startMusicTime;
            if (deltaTime >= notes.Peek().Time / tickPerSecond)
            {
                var note = notes.Dequeue();
                var intensity = note.Velocity;
                while (notes.Peek().Time == note.Time)
                {
                    var plusNote = notes.Dequeue();
                    intensity += plusNote.Velocity;
                }
                vfx.SetFloat(intensityId, Mathf.Clamp01(Mathf.Log10(note.Velocity)/2.4f));
                vfx.Play();
                var available = animators.FindAll(animator => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                var hachoir = available.FirstOrDefault();
                hachoir?.SetTrigger("Prepare");
                if (notes.Count == 0)
                {
                    StopCoroutine(coroutine);
                }
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    public void ParseMidi(string fileName, string extension = "mid")
    {
        MidiSequence sequence;
        using (Stream inputStream = File.OpenRead(Application.dataPath + $"/{fileName}.{extension}"))
        {
            sequence = MidiSequence.Open(inputStream);
        }

        Debug.Log($"Parse MIDI file: {fileName}");
        Debug.Log($"sequence ticksPerBeat: {sequence.TicksPerBeatOrFrame}");
        ticksPerBeat = sequence.TicksPerBeatOrFrame;
        Debug.Log($"sequence division: {sequence.Division}");
        foreach (var track in sequence.Tracks)
        {
            var time = 0f;
            Debug.Log($"track name: {track.TrackName}");
            foreach (var evt in track.Events)
            {
                switch (evt.GetType().ToString())
                {
                    case "MidiSharp.Events.Meta.TimeSignatureMetaMidiEvent":
                        {
                            var e = (evt as TimeSignatureMetaMidiEvent);
                            Debug.Log($"TIME event deltatime {e.DeltaTime}; numerator: {e.Numerator}; denominator: {e.Denominator};");
                        }
                        break;
                    case "MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent":
                        {
                            var e = (evt as NoteVoiceMidiEvent);
                            time += e.DeltaTime;
                            Debug.Log($"ON_NOTE event note {e.Note} {MidiEvent.GetNoteName(e.Note)}; time: {time}; velocity {e.Parameter2}");
                            notes.Enqueue(new Note
                            {
                                Time = time,
                                Velocity = e.Parameter2
                            });
                        }
                        break;
                    case "MidiSharp.Events.Voice.Note.OffNoteVoiceMidiEvent":
                        {
                            var e = (evt as NoteVoiceMidiEvent);
                            time += e.DeltaTime;
                            //Debug.Log($"OFF_NOTE event note {e.Note}; time: {time};");
                        }
                        break;
                    default:
                        {
                            Debug.Log(evt.GetType().ToString());
                        }
                        break;
                }
            }
        }
    }
}
