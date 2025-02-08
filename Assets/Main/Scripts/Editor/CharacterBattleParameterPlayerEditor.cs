using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBattleParameterPlayer))]
public class CharacterBattleParameterPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ターゲットとなるオブジェクトを取得
        CharacterBattleParameterPlayer character = (CharacterBattleParameterPlayer)target;

        // キャラクタータイプの選択フィールドを表示
        character.CharacterType = (CharacterBattleParameter.CharacterTypes)EditorGUILayout.EnumPopup("キャラクタータイプ", character.CharacterType);

        // キャラクタータイプに応じて表示するパラメータを切り替え
        EditorGUILayout.PropertyField(serializedObject.FindProperty("InitialBattleParameter"), new GUIContent("初期バトルパラメータ"));

        // 共通のバトルパラメータを表示
        EditorGUILayout.PropertyField(serializedObject.FindProperty("BattleParameter"), new GUIContent("バトルパラメータ"));

        // 変更を反映
        serializedObject.ApplyModifiedProperties();
    }
}