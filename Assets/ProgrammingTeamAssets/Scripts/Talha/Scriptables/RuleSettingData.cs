using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scriptables
{
    [CreateAssetMenu(fileName = "RuleSettingData", menuName = "Scriptables/Settings/RuleSettingData")]
    public class RuleSettingData : ScriptableObject
    {
        public StringData RuleID;
        public List<StringData> WinAgainst;
    }
}