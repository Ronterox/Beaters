using System.Collections.Generic;
using Managers;
using ScriptableObjects;

namespace Utilities
{
    public class RuneScriptableSelector : ScriptableSelector<ScriptableRune>
    {
        protected override List<ushort> GetObjectIds() => DataManager.GetRunesIds();

        protected override void SetObject()
        {
            ScriptableRune rune = m_PlayerObjects[m_Index];

            objectImage.sprite = rune.runeSprite;

            GameManager.PutRune(rune);
        }
    }
}
