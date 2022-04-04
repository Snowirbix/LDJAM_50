using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsRoll : MonoBehaviour
{
    public Animator Animator;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
        Animator.SetTrigger("Roll");
    }

}
