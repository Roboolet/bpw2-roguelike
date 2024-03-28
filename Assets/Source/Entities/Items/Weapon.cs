using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : Item
{
    public WeaponAttackData[] attackData;

    [System.Serializable]
    public struct WeaponAttackData
    {
        // the offset assumes a default attack is to the right
        public Vector2Int offsetFromUser;
        public int damage;
        public int startupDelay;
    }
}
