using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
    public Animator Animator;
    public Animator ImageFadeAnimator;
    public Animator TextFadeAnimator;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);
        TextFadeAnimator.SetTrigger("Fade");
        yield return new WaitForSeconds(8f);
        ImageFadeAnimator.SetTrigger("Fade"); 
        yield return new WaitForSeconds(7f);
        Animator.SetTrigger("Roll");
    }

}
