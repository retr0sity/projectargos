/*
using UnityEditor;
using UnityEngine;
using Game.Components;

[CustomEditor(typeof(Transform))]
public class SetChildTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Transform selectedTransform = (Transform)target;

        if (GUILayout.Button("Set Child Transform"))
        {
            SetChildTransform(selectedTransform);
        }
    }

    private void SetChildTransform(Transform ToolTransform)
    {
        var GO = new GameObject();
        GO.transform.SetParent(ToolTransform.parent.transform);
        GO.transform.position = ToolTransform.position;
        GO.transform.eulerAngles = ToolTransform.eulerAngles;
        GO.transform.localScale = ToolTransform.localScale;
        GO.name = ToolTransform.gameObject.name;
        var a = GO.AddComponent<ToolComponent>();
        a.ToolSettingID = ToolTransform.gameObject.name;

        ToolTransform.SetParent(GO.transform);

        // Set child's position, rotation, and scale
        ToolTransform.localPosition = Vector3.zero;
        ToolTransform.localRotation = Quaternion.identity;
        ToolTransform.localScale = Vector3.one;

        Debug.Log("Child transform set to zero position, zero rotation, and one scale.");
    }
}
*/