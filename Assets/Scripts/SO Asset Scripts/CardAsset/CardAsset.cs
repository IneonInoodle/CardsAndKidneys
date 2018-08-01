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

public enum CardType
{   
    Spell,
    Monster,
    Hp,
    Neutral,
    Player
}

public class CardAsset : ScriptableObject 
{
    // this object will hold the info about the most general card
    [Header("General info")]

    //public string Description;  
    // Description for spell or character

    public Sprite CardImage;
    public Sprite CardBodyImage;

    public Sprite CardArrowImage;

    public CardType Type;
    public Sprite FrameImage;

    [Header("Field Card Info")]
    public Sprite DamageImage;
    public int Damage; //if zero then a spell card
    public string MonsterScriptName;
    public int specialCreatureAmount;

    [Header("SpellInfo")]
    //public int ManaCost;

    public string CardTitelText;
    public string SpellScriptName;
    public int specialSpellAmount;
    public TargetingOptions Targets;

}
