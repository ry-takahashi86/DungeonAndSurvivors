using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObject), true)]
public class BattleParameterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクターを表示
        DrawDefaultInspector();

        // 対象が BattleParameter または BattleParameterPlayer の場合に処理を追加
        if (target is BattleParameter battleParameter)
        {
            GUILayout.Space(20);
            GUILayout.Label("計算パラメータ (BattleParameter)", EditorStyles.boldLabel);
            DisplayBattleData(battleParameter);
        }
        else if (target is BattleParameterPlayer battleParameterPlayer)
        {
            GUILayout.Space(20);
            GUILayout.Label("計算パラメータ (BattleParameterPlayer)", EditorStyles.boldLabel);
            DisplayBattleDataPlayer(battleParameterPlayer);
        }
    }

    /// <summary>
    /// BattleParameterBase のデータを表示
    /// </summary>
    /// <param name="data">BattleParameterBase</param>
    private void DisplayBattleData(BattleParameter battleParameter)
    {
        if (battleParameter != null)
        {
            EditorGUILayout.LabelField("MaxHP", battleParameter.Data.MaxHP.ToString());
            EditorGUILayout.LabelField("Physical Damage Bonus", battleParameter.Data.PhysicalDamageBonus.ToString());
            EditorGUILayout.LabelField("Critical Rate", battleParameter.Data.CriticalRate.ToString());
            EditorGUILayout.LabelField("Skill Cooldown Rate", battleParameter.Data.SkillCooldownRate.ToString());
            EditorGUILayout.LabelField("Move Speed Bonus", battleParameter.Data.MoveSpeedBonus.ToString());
            EditorGUILayout.LabelField("Attack Speed", battleParameter.Data.AttackSpeed.ToString());
            EditorGUILayout.LabelField("Physical Damage Reduction Rate", battleParameter.Data.PhysicalDamageReductionRate.ToString());
            EditorGUILayout.LabelField("MaxMP", battleParameter.Data.MaxMP.ToString());
            EditorGUILayout.LabelField("Magic Damage Bonus", battleParameter.Data.MagicDamageBonus.ToString());
            EditorGUILayout.LabelField("Magic Damage Reduction Rate", battleParameter.Data.MagicDamageReductionRate.ToString());
        }
    }

    /// <summary>
    /// BattleParameterBase のデータを表示
    /// </summary>
    /// <param name="data">BattleParameterBase</param>
    private void DisplayBattleDataPlayer(BattleParameterPlayer battleParameter)
    {
        if (battleParameter != null)
        {
            EditorGUILayout.LabelField("MaxHP", battleParameter.Data.MaxHP.ToString());
            EditorGUILayout.LabelField("Physical Damage Bonus", battleParameter.Data.PhysicalDamageBonus.ToString());
            EditorGUILayout.LabelField("Critical Rate", battleParameter.Data.CriticalRate.ToString());
            EditorGUILayout.LabelField("Skill Cooldown Rate", battleParameter.Data.SkillCooldownRate.ToString());
            EditorGUILayout.LabelField("Move Speed Bonus", battleParameter.Data.MoveSpeedBonus.ToString());
            EditorGUILayout.LabelField("Attack Speed", battleParameter.Data.AttackSpeed.ToString());
            EditorGUILayout.LabelField("Physical Damage Reduction Rate", battleParameter.Data.PhysicalDamageReductionRate.ToString());
            EditorGUILayout.LabelField("MaxMP", battleParameter.Data.MaxMP.ToString());
            EditorGUILayout.LabelField("Magic Damage Bonus", battleParameter.Data.MagicDamageBonus.ToString());
            EditorGUILayout.LabelField("Magic Damage Reduction Rate", battleParameter.Data.MagicDamageReductionRate.ToString());
        }
    }
}