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
    public record Track
    {
        public Queue<Note> notes = new();
    }
    [Serializable]
    public record Note
    {
        public float Time { get; set; }
        public int Value { get; set; }
        public float Velocity { get; set; }
    }
    public List<Track> tracks = new ();
    [Obsolete]
    public Queue<Note> notes = new();
    [ReadOnly]
    public int ticksPerBeat;

    public float bpm = 160f;
    public float bps;
    public float tickPerSecond;

    public VisualEffect vfx;

    private float startMusicTime;

    public float musicTimer;

    public float attackDelay = 0.75f;

    public float timeReserved = 1.2f;

    private IEnumerator coroutine;

    private int intensityId;

    [Serializable]
    public record Hachoir
    {
        public int index;
        public Animator animator;
        public bool reserved = false;
        public int reservedByTrack;

    }
    public List<Hachoir> hachoirs = new();
    private AudioSource audio;

    public string[] midiFiles = new [] { "Music/BASS_MAIN_MIDI" };

    private void Start()
    {
        intensityId = Shader.PropertyToID("Intensity");
        bps = bpm / 60f;
        var animators = GetComponentsInChildren<Animator>().ToList();
        hachoirs = animators.Select((animator, index) => new Hachoir
        {
            index = index,
            animator = animator
        }).ToList();
        audio = this.Q<AudioSource>();
        foreach(var midiFile in midiFiles)
        {
            var track = new Track();
            tracks.Add(track);
            ParseMidi(track, midiFile);
        }
        tickPerSecond = bps * ticksPerBeat;
        audio.Play();
        startMusicTime = (float)AudioSettings.dspTime;
        coroutine = CheckTheBeat();
        StartCoroutine(coroutine);
    }

    public void Pause()
    {
        var animators = hachoirs.Select(h => h.animator).ToList();
        animators
            .FindAll(animator => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            .ForEach(animator => animator.SetBool("Killed", true));
        
        animators
            .FindAll(animator => !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            .ForEach(animator => animator.speed = 0);
    }

    IEnumerator CheckTheBeat()
    {
        // kick et clap qui avance
        // 
        yield return null;
        while (true)
        {
            musicTimer = (float)AudioSettings.dspTime - startMusicTime;
            foreach (var track in tracks)
            {
                if (musicTimer + attackDelay >= (track.notes.Peek().Time / tickPerSecond))
                {
                    var note = track.notes.Dequeue();
                    var velocity = note.Velocity;
                    while (track.notes.Peek().Time == note.Time)
                    {
                        var plusNote = track.notes.Dequeue();
                        velocity += plusNote.Velocity;
                    }
                    vfx.Play();
                    var available = hachoirs.FindAll(h => h.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
                    //never use all of them
                    if (available.Count > 2)
                    {
                        var hachoir = available.FirstOrDefault();
                        hachoir?.animator.SetTrigger("Prepare");
                        //var hachoir2 = available.ElementAtOrDefault(1);
                        //hachoir2?.SetTrigger("Prepare");
                    }
                    if (track.notes.Count == 0)
                    {
                        StopCoroutine(coroutine);
                    }
                }
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    public void ParseMidi(Track track, string fileName, string extension = "mid")
    {
        MidiSequence sequence;
        using (Stream inputStream = File.OpenRead(Application.dataPath + $"/{fileName}.{extension}"))
        {
            sequence = MidiSequence.Open(inputStream);
        }

        Debug.Log($"Parse MIDI file: {fileName}");
        Debug.Log($"sequence ticksPerBeat: {sequence.TicksPerBeatOrFrame}");
        ticksPerBeat = sequence.TicksPerBeatOrFrame;
        //Debug.Log($"sequence division: {sequence.Division}");
        foreach (var t in sequence.Tracks)
        {
            var time = 0f;
            Debug.Log($"track name: {t.TrackName}");
            foreach (var evt in t.Events)
            {
                switch (evt.GetType().ToString())
                {
                    case "MidiSharp.Events.Meta.TimeSignatureMetaMidiEvent":
                        {
                            var e = (evt as TimeSignatureMetaMidiEvent);
                            //Debug.Log($"TIME event deltatime {e.DeltaTime}; numerator: {e.Numerator}; denominator: {e.Denominator};");
                        }
                        break;
                    case "MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent":
                        {
                            var e = (evt as NoteVoiceMidiEvent);
                            time += e.DeltaTime;
                            //Debug.Log($"ON_NOTE event note {e.Note} {MidiEvent.GetNoteName(e.Note)}; time: {time}; velocity {e.Parameter2}");
                            track.notes.Enqueue(new Note
                            {
                                Time = time,
                                Value = e.Note,
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
                            //Debug.Log(evt.GetType().ToString());
                        }
                        break;
                }
            }
        }
    }
}
