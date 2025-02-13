using MoreMountains.TopDownEngine;

public class GoToLevelNextFloor : ButtonActivated
{
    /// <summary>
    /// アクションのトリガー条件
    /// </summary>
    public override void TriggerButtonAction()
    {
        if (!CheckNumberOfUses())
        {
            return;
        }
        base.TriggerButtonAction ();

        GoToNextLevel();
    }

    /// <summary>
    /// 次のフロアに移動する
    /// </summary>
    public virtual void GoToNextLevel()
    {
        DungeonManager.Instance.AdvanceFloor();
    }
}