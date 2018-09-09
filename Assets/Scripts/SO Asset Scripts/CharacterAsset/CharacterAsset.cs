using UnityEngine;
using System.Collections;

public enum CharClass{ Elf, Monk, Warrior}

public class CharacterAsset : ScriptableObject 
{
    /*
    public CharClass Class;
	public string ClassName;
	public int MaxHealth = 30;
	public string HeroPowerName;
    */

    public Sound[] CaptureSounds;
    public Sound[] DamageSounds;
    public Sound[] HpupSounds;
    public Sound[] KidneyStealSounds;
    public Sound[] LinesSounds;
    public Sound[] NeutralSounds;

    public CardAsset playerCardAsset;
    public Sprite SpellCardBackgroundSprite;
    public Material SpellCardFrameMat;

    public Sprite PortraitFull;
    public Sprite PortaitHalf;

    /*
    public Sprite AvatarImage;
    public Sprite HeroPowerIconImage;
    public Sprite AvatarBGImage;
    public Sprite HeroPowerBGImage;
    public Color32 AvatarBGTint;
    public Color32 HeroPowerBGTint;
    public Color32 ClassCardTint;
    public Color32 ClassRibbonsTint;
    */    
}
