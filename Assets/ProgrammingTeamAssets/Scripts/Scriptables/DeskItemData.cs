using UnityEngine;

[CreateAssetMenu(fileName = "DeskItemData", menuName = "Scriptable Objects/DeskItemData")]
public class DeskItemData : ScriptableObject
{
    public string ID;
    public string Description;
    public bool IsConsole;
}
