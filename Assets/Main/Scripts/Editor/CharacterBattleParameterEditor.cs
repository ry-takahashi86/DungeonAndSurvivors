using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBattleParameter))]
public class CharacterBattleParameterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ターゲットとなるオブジェクトを取得
        CharacterBattleParameter character = (CharacterBattleParameter)target;

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