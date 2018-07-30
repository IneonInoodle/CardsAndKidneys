using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetingOptions
{
    NoTarget,
    AllFieldCards, 
    AllCreatures,
    AllPlayers, 
    EnemyPlayer, 
    YourPlayer
}

public class CardAsset : ScriptableObject 
{
    // this object will hold the info about the most general card
    [Header("General info")]
    public CharacterAsset characterAsset;  // if this is null, it`s a neutral card
    //public string Description;  
    // Description for spell or character

    public Sprite CardImage; 

    [Header("Field Card Info")]
    public int Damage; //if zero then a spell card

    public Sprite CardTypeImage; //if card damage is 0 spell card
    public int Multiplyer;
    public int Hp;
    public int ActionPoints;

    public string CreatureScriptName;
    public int specialCreatureAmount;

    [Header("SpellInfo")]
    //public int ManaCost;

    public string CardTitelText;
    public string SpellScriptName;
    public int specialSpellAmount;
    public TargetingOptions Targets;

}
