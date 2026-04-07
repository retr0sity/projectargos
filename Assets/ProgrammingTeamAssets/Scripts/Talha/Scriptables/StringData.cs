using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Scriptables
{
    [CreateAssetMenu(fileName ="StringData", menuName = "Scriptables/Settings/StringData")]
    public class StringData : ScriptableObject
    {
        public string ID;
    }
}