using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationScript : MonoBehaviour
{

    public Animator ThreeAnimator;
    public Animator TwoAnimator;
    public Animator OneAnimator;
    public Animator FightAnimator;

    public GameObject Three;
    public GameObject Two;
    public GameObject One;
    public GameObject Fight;

    public string FightSceneName;

    public AudioSource OneAudio;
    public AudioSource TwoAudio;
    public AudioSource ThreeAudio;
    public AudioSource FightAudio;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Delay(0.5f));
    }

    IEnumerator Delay(float time)
    {
        Three.SetActive(true);
        ThreeAudio.Play();
        yield return new WaitForSeconds(time);
        ThreeAnimator.SetTrigger("Slide");
        yield return new WaitForSeconds(time);
        Two.SetActive(true);
        TwoAudio.Play();
        yield return new WaitForSeconds(time);
        TwoAnimator.SetTrigger("Slide");
        yield return new WaitForSeconds(time);
        One.SetActive(true);
        OneAudio.Play();
        yield return new WaitForSeconds(time);
        OneAnimator.SetTrigger("Slide");
        FightAnimator.SetTrigger("Slide");
        yield return new WaitForSeconds(time);
        FightAudio.Play();
        yield return new WaitForSeconds(time * 2);
        SceneManager.LoadScene(FightSceneName);
    }


}
