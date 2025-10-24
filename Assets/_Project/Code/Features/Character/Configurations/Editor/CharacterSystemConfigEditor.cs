using UnityEditor;

namespace _Project.Code.Features.Character.Configurations.Editor
{
    [CustomEditor(typeof(CharacterSystemConfig), true)]
    public class CharacterSystemConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var config = (CharacterSystemConfig)target;

            // Тип системы
            var type = config.CharacterSystemType;
            EditorGUILayout.LabelField("System Type", type != null ? type.Name : "Unknown", EditorStyles.boldLabel);

            // Отрисовка остальных полей
            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
