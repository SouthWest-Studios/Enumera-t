using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomStartAnimation : MonoBehaviour
{
    Animator anim;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.Update(0f);                 
        float t = Random.value;          

        if (!string.IsNullOrEmpty(""))
            anim.Play(Animator.StringToHash(""), 0, t);
        else
        {
            var info = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(info.fullPathHash, 0, t);
        }
    }
}
