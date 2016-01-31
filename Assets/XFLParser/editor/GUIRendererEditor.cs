using renderer;
using UnityEditor;
using UnityEngine;
namespace editor
{
    [CustomEditor(typeof(GUIRenderer))]
    class GUIRendererEditor:Editor
    {
        int selectedQueue;
        void OnEnable()
        {
            GUIRenderer renderer = (GUIRenderer)target;
            if (renderer.library == null)
            {
                selectedQueue = 0;
            }
            else
            {
                selectedQueue = Mathf.Max(0, renderer.library.symbolNames.IndexOf(serializedObject.FindProperty("symbolName").stringValue));
                
            }
            renderer.symbolName = renderer.library.symbolNames[selectedQueue];
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUIRenderer renderer = (GUIRenderer)target;
            if (renderer.library == null)
            {
                EditorGUILayout.LabelField("Assign Library Object");
            }
            else
            {
                if (renderer.library.symbolNames.Count > 0)
                {
                    int _selectedQueue = EditorGUILayout.Popup("Symbol: ", selectedQueue, renderer.library.symbolNames.ToArray());
                    if (_selectedQueue != selectedQueue)
                    {
                        selectedQueue = _selectedQueue;
                        serializedObject.FindProperty("symbolName").stringValue = renderer.library.symbolNames[selectedQueue];
                        serializedObject.ApplyModifiedProperties();
                    }
                    if(GUILayout.Button("load symbol"))
                    {
                        
                        renderer.load();
                    }
                    if (serializedObject.FindProperty("maxFrames").intValue > 0)
                    {
                        float currentFrame = serializedObject.FindProperty("currentFrame").floatValue;
                        float _nextFrame = GUILayout.HorizontalSlider(currentFrame, 0, serializedObject.FindProperty("maxFrames").intValue);
                        if (currentFrame != _nextFrame)
                        {
                            currentFrame = _nextFrame;
                            serializedObject.FindProperty("currentFrame").floatValue = currentFrame;
                            serializedObject.ApplyModifiedProperties();
                        }
                        if (serializedObject.FindProperty("isPlaying").boolValue)
                        {
                            if (GUILayout.Button("Pause"))
                            {
                                serializedObject.FindProperty("isPlaying").boolValue = false;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Play"))
                            {
                                serializedObject.FindProperty("isPlaying").boolValue = true;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    
                }

                else
                {
                    EditorGUILayout.LabelField("No symbols on the Library Object");
                }
            }
        }
    }
}
