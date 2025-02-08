using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

/// <summary>
/// MobileGameManagerは、GameManagerの機能に加えて、
/// ・アプリ開始時（Start）に「Load」イベントを発行し、保存済みデータをロード
/// ・アプリがバックグラウンドへ移行（OnApplicationPause(true)）および終了時（OnApplicationQuit）に「Save」イベントを発行
/// ・保存形式は、MMSaveLoadManagerMethod　で設定しています
/// </summary>
public class GameManagerMobile : GameManager
{
    private MMGameEvent gameEvent = new MMGameEvent();

    /// <summary>
    /// Startで基本の初期化後、セーブファイルからロードを実行する
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // アプリ開始時にロードイベントを発行（保存データがあれば各システムが復元される）
        gameEvent.EventName = "Load";
        MMEventManager.TriggerEvent(gameEvent);
    }

    /// <summary>
    /// モバイル環境では、アプリがバックグラウンドに移行する際にセーブすることが重要。
    /// OnApplicationPauseで、pauseStatusがtrueの場合にセーブ、falseの場合に必要ならロードを実行する
    /// </summary>
    /// <param name="pauseStatus">trueならアプリが一時停止（バックグラウンド）、falseなら復帰</param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // アプリがバックグラウンドに移行する際にセーブ
            gameEvent.EventName = "Save";
            MMEventManager.TriggerEvent(gameEvent);
        }
        else
        {
            // アプリ復帰時にロード（必要に応じて実装、ここでは最新状態に更新）
            gameEvent.EventName = "Load";
            MMEventManager.TriggerEvent(gameEvent);
        }
    }

    /// <summary>
    /// アプリ終了時にもセーブを実行する
    /// </summary>
    private void OnApplicationQuit()
    {
        gameEvent.EventName = "Save";
        MMEventManager.TriggerEvent(gameEvent);
    }
}