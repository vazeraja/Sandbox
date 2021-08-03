using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class ReanimatorSettings : ScriptableObject {
        private const string k_MyCustomSettingsPath = "Assets/Reanimator/Editor/Settings/MyCustomSettings.asset";

        [SerializeField] private int m_Number;

        [SerializeField] private string m_SomeString;

        internal static ReanimatorSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ReanimatorSettings>(k_MyCustomSettingsPath);
            if (settings == null) {
                settings = CreateInstance<ReanimatorSettings>();
                settings.m_Number = 42;
                settings.m_SomeString = "The answer to the universe";
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    // Create MyCustomSettingsProvider by deriving from SettingsProvider:
    class MyCustomSettingsProvider : SettingsProvider {
        private SerializedObject m_CustomSettings;

        private class Styles {
            public static readonly GUIContent number = new GUIContent("My Number");
            public static readonly GUIContent someString = new GUIContent("Some string");
        }

        private const string k_MyCustomSettingsPath = "Assets/Reanimator/Editor/Settings/MyCustomSettings.asset";

        private MyCustomSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        { }

        public static bool IsSettingsAvailable() => File.Exists(k_MyCustomSettingsPath);

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = ReanimatorSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Number"), Styles.number);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_SomeString"), Styles.someString);
            m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (IsSettingsAvailable()) {
                var provider = new MyCustomSettingsProvider("Project/MyCustomSettingsProvider", SettingsScope.Project) {
                    keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
                };

                // Automatically extract all keywords from the Styles.
                return provider;
            }

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
    }
}