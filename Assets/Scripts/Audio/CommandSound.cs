using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public abstract class CommandSound
    {
        public abstract void Execute();
    }

    public class PlayerDamage : CommandSound
    {
        public override void Execute()
        {
            AudioManager.instance.PlaySound("Damage");
        }
    }
    public class PlayerHpUp : CommandSound
    {
        public override void Execute()
        {
            AudioManager.instance.PlaySound("Damage");
        }
    }
}