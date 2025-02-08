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
            EditorGUILayout.LabelField("MaxHP", battleParameter.MaxHP.ToString());
            EditorGUILayout.LabelField("Physical Damage Bonus", battleParameter.PhysicalDamageBonus.ToString());
            EditorGUILayout.LabelField("Critical Rate", battleParameter.CriticalRate.ToString());
            EditorGUILayout.LabelField("Skill Cooldown Rate", battleParameter.SkillCooldownRate.ToString());
            EditorGUILayout.LabelField("Move Speed", battleParameter.MoveSpeed.ToString());
            EditorGUILayout.LabelField("Attack Speed", battleParameter.AttackSpeed.ToString());
            EditorGUILayout.LabelField("Physical Damage Reduction Rate", battleParameter.PhysicalDamageReductionRate.ToString());
            EditorGUILayout.LabelField("MaxMP", battleParameter.MaxMP.ToString());
            EditorGUILayout.LabelField("Magic Damage Bonus", battleParameter.MagicDamageBonus.ToString());
            EditorGUILayout.LabelField("Magic Damage Reduction Rate", battleParameter.MagicDamageReductionRate.ToString());
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
            EditorGUILayout.LabelField("MaxHP", battleParameter.MaxHP.ToString());
            EditorGUILayout.LabelField("Physical Damage Bonus", battleParameter.PhysicalDamageBonus.ToString());
            EditorGUILayout.LabelField("Critical Rate", battleParameter.CriticalRate.ToString());
            EditorGUILayout.LabelField("Skill Cooldown Rate", battleParameter.SkillCooldownRate.ToString());
            EditorGUILayout.LabelField("Move Speed", battleParameter.MoveSpeed.ToString());
            EditorGUILayout.LabelField("Attack Speed", battleParameter.AttackSpeed.ToString());
            EditorGUILayout.LabelField("Physical Damage Reduction Rate", battleParameter.PhysicalDamageReductionRate.ToString());
            EditorGUILayout.LabelField("MaxMP", battleParameter.MaxMP.ToString());
            EditorGUILayout.LabelField("Magic Damage Bonus", battleParameter.MagicDamageBonus.ToString());
            EditorGUILayout.LabelField("Magic Damage Reduction Rate", battleParameter.MagicDamageReductionRate.ToString());
        }
    }
}