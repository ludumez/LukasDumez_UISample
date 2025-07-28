using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

[CustomEditor(typeof(SelectableElement), true)]
public class InspeactableElementInspector : ButtonEditor
{
    //Override class that handles any new property added to a class that inherits f4rom a base class
    //Allows the inspector to display properties that are not part of the base class (Button in this case)


    private List<SerializedProperty> _newProperties = new List<SerializedProperty>();

    protected override void OnEnable()
    {
        base.OnEnable();
        _newProperties.Clear();

        // Get properties of the derived class (excluding base Button properties)
        var targetType = target.GetType();
        var baseType = typeof(Button);

        SerializedProperty iterator = serializedObject.GetIterator();

        // Ensure the iterator starts properly
        if (iterator != null && iterator.NextVisible(true))
        {
            do
            {
                if (IsNewProperty(targetType, baseType, iterator.name))
                {
                    _newProperties.Add(serializedObject.FindProperty(iterator.name));
                }
            }
            while (iterator.NextVisible(false));  // Iterate through all serialized properties
        }
    }

    public override void OnInspectorGUI()
    {
        //Draw Unity's default Button inspector first
        base.OnInspectorGUI();


        // Draw a separator for clarity
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Properties", EditorStyles.boldLabel);

        // Draw only the new serialized properties from the derived class
        serializedObject.Update();
        foreach (var prop in _newProperties)
        {
            EditorGUILayout.PropertyField(prop);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private bool IsNewProperty(System.Type derived, System.Type baseType, string propertyName)
    {
        // Check if the property exists in the base class (Button)
        return derived.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null &&
               baseType.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null;
    }
}

