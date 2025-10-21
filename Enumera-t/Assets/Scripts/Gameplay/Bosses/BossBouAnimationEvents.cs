using UnityEngine;
using static GameplayManager;

public class BossBouAnimationEvents : MonoBehaviour
{
    public IBossBehavior bossLogic;

    public void OnSwallQuarter()
    {
        if (bossLogic != null)
        {
            (bossLogic as BossBou)?.OnSwallQuarter();
        }
        else
        {
            Debug.LogWarning("BossBouAnimationEvents: bossBouLogic no asignado");
        }
    }
}
