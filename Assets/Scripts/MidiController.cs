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

    public VisualEffect vfx;

    private float startTime;

    public float timeSpent;

    private IEnumerator coroutine;

    private int intensityId;

    private void Start()
    {
        ParseMidi("cle_velocite_rythm");
        startTime = Time.time;
        coroutine = CheckTheBeat();
        StartCoroutine(coroutine);
        intensityId = Shader.PropertyToID("Intensity");
    }

    IEnumerator CheckTheBeat()
    {
        while (true)
        {
            var deltaTime = Time.time - startTime;
            timeSpent = deltaTime;
            if (deltaTime >= notes.Peek().Time / ticksPerBeat)
            {
                var note = notes.Dequeue();
                vfx.SetFloat(intensityId, note.Velocity/128f);
                vfx.Play();
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
