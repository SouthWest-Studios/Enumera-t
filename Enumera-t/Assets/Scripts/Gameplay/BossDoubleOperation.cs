using UnityEngine;
using static GameplayManager;

public class BossDoubleOperation : IBossBehavior
{
    private GameplayManager manager;
    private int correctAnswers = 0;

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
    }

    public void OnCorrectAnswer()
    {
        correctAnswers++;

        if (correctAnswers >= 2)
        {
            manager.health -= 2;
            manager.healthBar.fillAmount = manager.health / 10f;
            correctAnswers = 0;
        }

        manager.RoundCompleted();
    }

    public void OnWrongAnswer() { }

    public void Update() { }
}
