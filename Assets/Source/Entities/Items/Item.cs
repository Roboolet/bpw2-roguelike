using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public ItemRarity rarity;

    public enum ItemRarity
    {
        Basic, Rare
    }
}
