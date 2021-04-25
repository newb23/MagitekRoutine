﻿using System;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.BlackMage;
using Magitek.Utilities;

namespace Magitek.Logic.BlackMage
{
    internal static class Buff
    {
        public static async Task<bool> Triplecast()
        {
            if (Core.Me.ClassLevel < Spells.Triplecast.LevelAcquired)
                return false;

            if (!BlackMageSettings.Instance.TripleCast)
                return false;

            if (Spells.Triplecast.Cooldown != TimeSpan.Zero)
                return false;

            if (Casting.LastSpell == Spells.Xenoglossy || Casting.LastSpell == Spells.Fire3 || Casting.LastSpell == Spells.Blizzard3)
                return await Spells.Triplecast.Cast(Core.Me);

            if (ActionResourceManager.BlackMage.UmbralHearts == 3 && Casting.LastSpell == Spells.Fire3)
                return await Spells.Triplecast.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> Enochian()
        {
            //First, check if we can even cast Enochian
            if (Core.Me.ClassLevel < Spells.Enochian.LevelAcquired)
                return false;

            //Then, if we have it, we don't need to cast it again
            if (Core.Me.HasEnochian())
                return false;

            //Obviously we can't use it if it's on cooldown
            if (Spells.Enochian.Cooldown != TimeSpan.Zero)
                return false;

            //Enochian requires at least one Astral or Umbral stack
            if (ActionResourceManager.BlackMage.AstralStacks == 0
                && ActionResourceManager.BlackMage.UmbralStacks == 0)
                return false;

            return await Spells.Enochian.Cast(Core.Me);
        }

        public static async Task<bool> Sharpcast()
        {
            if (Core.Me.ClassLevel < Spells.Sharpcast.LevelAcquired)
                return false;

            if (Spells.Sharpcast.Cooldown != TimeSpan.Zero)
                return false;

            if (!BlackMageSettings.Instance.Sharpcast)
                return false;
            // If we used something that opens the GCD

            if (Casting.LastSpell == Spells.Fire3
                || Casting.LastSpell == Spells.Blizzard3
                || Casting.LastSpell == Spells.Thunder3
                || Core.Me.HasAura(Auras.Triplecast)) 
                return await Spells.Sharpcast.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> LeyLines()
        {
            if (Core.Me.ClassLevel < Spells.LeyLines.LevelAcquired)
                return false;

            if (Spells.LeyLines.Cooldown != TimeSpan.Zero)
                return false;

            if (!BlackMageSettings.Instance.LeyLines)
                return false;

            if (BlackMageSettings.Instance.LeyLinesBossOnly
                && !Core.Me.CurrentTarget.IsBoss())
                return false;

            //Don't use while moving
            if (MovementManager.IsMoving)
                return false;

            // Do not Ley Lines if we don't have 3 astral stacks
            if (ActionResourceManager.BlackMage.AstralStacks != 3
                || ActionResourceManager.BlackMage.UmbralStacks > 0)
                return false;

            // Do not Ley Lines if we don't have any umbral hearts (roundabout check to see if we're at the begining of astral)
            if (Casting.LastSpell == Spells.Fire3
                && ActionResourceManager.BlackMage.UmbralHearts == 3 
                || Core.Me.HasAura(Auras.Triplecast))
                // Fire 3 is always used at the start of Astral
                return await Spells.LeyLines.Cast(Core.Me);
            // If we used something that opens the GCD
            // Fire3 caused this to go off at the beginning of astral anyway
            //if (Casting.LastSpell == Spells.Blizzard3)// Thunder3 only opens up the GCD if it is using Thundercloud || Casting.LastSpell == Spells.Thunder3 || Core.Me.HasAura(Auras.Triplecast) || Casting.LastSpell == Spells.Xenoglossy)
            //return await Spells.LeyLines.Cast(Core.Me);
            
            return false;
        }

        public static async Task<bool> UmbralSoul()
        {
            if (Core.Me.ClassLevel < Spells.UmbralSoul.LevelAcquired)
                return false;

            if (Spells.UmbralSoul.Cooldown != TimeSpan.Zero)
                return false;

            if (!Core.Me.HasEnochian())
                return false;

            // Do not Umbral Soul unless we have 1 umbral stack
            if (ActionResourceManager.BlackMage.UmbralStacks != 1)
                return false;

            return await Spells.UmbralSoul.Cast(Core.Me);
        }

        public static async Task<bool> ManaFont()
        {
            if (Core.Me.ClassLevel < Spells.ManaFont.LevelAcquired)
                return false;

            if (Spells.ManaFont.Cooldown != TimeSpan.Zero)
                return false;

            if (!BlackMageSettings.Instance.ConvertAfterFire3)
                return false;

            if (Casting.LastSpell == Spells.Fire3
                && Spells.Fire.Cooldown.TotalMilliseconds > Globals.AnimationLockMs)
                return await Spells.ManaFont.Cast(Core.Me);

            return false;
        }
        public static async Task<bool> Transpose()
        {
            if (Core.Me.ClassLevel < Spells.Transpose.LevelAcquired)
                return false;

            if (Spells.Transpose.Cooldown != TimeSpan.Zero)
                return false;

            if (Core.Me.ClassLevel < 40
                && Core.Me.CurrentMana < 1600
                && ActionResourceManager.BlackMage.AstralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            if (Core.Me.ClassLevel < 40
                && Core.Me.CurrentMana == 10000
                && ActionResourceManager.BlackMage.UmbralStacks > 0)
                return await Spells.Transpose.Cast(Core.Me);

            return false;
        }

    }

}
