using UnityEngine;
using static GameplayManager;

public class BossDracAnimationEvents : MonoBehaviour
{
    public IBossBehavior bossLogic;


    public void windBossSound()
    {
        AudioManager.Instance.PlaywindBoss();
    }

}
