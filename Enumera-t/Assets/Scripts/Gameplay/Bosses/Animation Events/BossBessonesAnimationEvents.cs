using UnityEngine;
using static GameplayManager;

public class BossBessonesAnimationEvents : MonoBehaviour
{
    public IBossBehavior bossLogic;



    public void windBossSound()
    {
        AudioManager.Instance.PlaywindBoss();
    }

}
