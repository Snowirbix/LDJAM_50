using UnityEngine;
using MidiSharp;
using System.IO;
using MidiSharp.Events.Voice.Note;
using MidiSharp.Events.Meta;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
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
    public string kickFile;
    public Track kickTrack = new();
    public float kickTime;
    public string harmonyFile;
    public Track harmonyTrack = new();
    public string randomFile;
    public Track randomTrack = new();
    [ReadOnly]
    public int ticksPerBeat;

    public float bpm = 160f;
    public float bps;
    public float tickPerSecond;

    public VisualEffect vfx;

    private float startMusicTime;

    public float musicTimer;

    public float attackDelay = 0.75f;

    public float reloadDelay = 1.5f;

    private IEnumerator coroutine;

    private int intensityId;

    [Serializable]
    public record Hachoir
    {
        public int index;
        public Animator animator;
        public float lastAttack = 0f;
    }

    public List<Hachoir> hachoirs = new();
    private AudioSource audio;

    private int kickHachoir = 0;

    private System.Random rand = new ();

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

        ParseMidi(kickTrack, kickFile);
        ParseMidi(harmonyTrack, harmonyFile);
        //ParseMidi(randomTrack, randomFile);

        tickPerSecond = bps * ticksPerBeat;
        audio.Play();
        startMusicTime = (float)AudioSettings.dspTime;
        coroutine = CheckTheBeat();
        StartCoroutine(coroutine);
    }

    public void Pause()
    {
        var animators = hachoirs.Select(h => h.animator).ToList();
        var idle = animators.FindAll(animator => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        idle.ForEach(animator => animator.SetBool("Killed", true));
        idle.ForEach(animator => animator.speed = 0.1f);

        animators
            .FindAll(animator => !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            .ForEach(animator => animator.speed = 0f);
    }

    IEnumerator CheckTheBeat()
    {
        yield return null;
        while (true)
        {
            musicTimer = (float)AudioSettings.dspTime - startMusicTime;

            if (kickTrack.notes.Count > 0 && musicTimer < kickTime)
            {
                if (musicTimer + attackDelay >= (kickTrack.notes.Peek().Time / tickPerSecond))
                {
                    var note = kickTrack.notes.Dequeue();
                    var h = hachoirs[kickHachoir];
                    h?.animator.SetTrigger("Prepare");
                    kickHachoir = (kickHachoir + 1) % hachoirs.Count;
                    var h2 = hachoirs[kickHachoir];
                    h2?.animator.SetTrigger("Prepare");
                    kickHachoir = (kickHachoir + 1) % hachoirs.Count;
                    while (kickTrack.notes.Peek().Time == note.Time)
                    {
                        kickTrack.notes.Dequeue();
                    }
                    //while (harmonyTrack.notes.Peek().Time <= note.Time)
                    //{
                    //    // drop harmony notes past
                    //    var n = harmonyTrack.notes.Dequeue();
                    //}
                }
            }
            else if (harmonyTrack.notes.Count > 0)
            {
                if (musicTimer + attackDelay >= (harmonyTrack.notes.Peek().Time / tickPerSecond))
                {
                    var note = harmonyTrack.notes.Dequeue();
                    var h = hachoirs[kickHachoir];
                    h?.animator.SetTrigger("Prepare");
                    kickHachoir = (kickHachoir + 2) % hachoirs.Count;
                    while (harmonyTrack.notes.Peek().Time == note.Time)
                    {
                         harmonyTrack.notes.Dequeue();
                    }
                }
            }

            if (musicTimer >= 85)
            {
                EndGame();
                StopCoroutine(coroutine);
            }
            
            if (kickTrack.notes.Count == 0 && harmonyTrack.notes.Count == 0 && randomTrack.notes.Count == 0)
            {
                EndGame();
                StopCoroutine(coroutine);
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("321Fight");
    }

    public void ParseMidi(Track track, string fileName)
    {
        MidiSequence sequence;
        var midi = Resources.Load(fileName) as TextAsset;
        using (Stream stream = new MemoryStream(midi.bytes))
        {
            sequence = MidiSequence.Open(stream);
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
