using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionCanvas : MonoBehaviour
{
    private Animator anim;
    public TransitionCanvas instance;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void DoTransition(string level)
    {
        StartCoroutine(DoTransitionRoutine(level));
    }

    private IEnumerator DoTransitionRoutine(string level)
    {

        anim.Play("FadeIn", 0, 0f);

        int fadeInHash = Animator.StringToHash("FadeIn");
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).shortNameHash != fadeInHash)
            yield return null;

        while (anim.IsInTransition(0) || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        SceneManager.LoadScene(level);
    }
}
