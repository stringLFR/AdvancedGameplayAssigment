using UnityEngine;


public abstract class BuffORDebuffBase
{
    protected ICombatObject controller;
    protected float startingMana;
    protected DroneUnitBody targetHost;
    protected float manaDrainPerSec = 1f;

    public DroneUnitBody Host => targetHost;

    public virtual void InitBuffDebuff(ICombatObject c)
    {
        controller = c;
    }

    public virtual void AttachBuffDebuff(float mana, DroneUnitBody target)
    {
        targetHost = target;
        startingMana = mana;
        SetupBuffDebuff();
    }

    public virtual bool BuffDebuffDuration()
    {
        startingMana -= Time.deltaTime * manaDrainPerSec;

        if (startingMana < 0.1f)
        {
            CombatListener.AddLineToCombatText($"BuffDebuff ran out of mana!");

            EndBuffDebuff();

            return false;
        }

        return true;
    }

    protected abstract void EndBuffDebuff();

    protected abstract void SetupBuffDebuff();
}

#region buffs

public class OverdriveBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.Overdrive--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.Overdrive++;

        manaDrainPerSec = 1f;
    }
}

public class MartialProwessBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.MartialProwess--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.MartialProwess++;

        manaDrainPerSec = 1f;
    }
}

public class MagicalProwessBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.MagicalProwess--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.MagicalProwess++;

        manaDrainPerSec = 1f;
    }
}

public class ArmorPolishBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.ArmorPolish--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.ArmorPolish++;

        manaDrainPerSec = 1f;
    }
}

public class ManaReinforcementBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.ManaReinforcement--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.ManaReinforcement++;

        manaDrainPerSec = 1f;
    }
}

public class CriticalProtectionBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.CriticalProtection--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.CriticalProtection++;

        manaDrainPerSec = 1f;
    }
}

public class MultiHitsBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.MultiHits--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.MultiHits++;

        manaDrainPerSec = 1f;
    }
}

public class ManaRegenerationBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.ManaRegeneration--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.ManaRegeneration++;

        manaDrainPerSec = 1f;
    }
}

public class HealthRegenerationBuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.HealthRegeneration--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.HealthRegeneration++;

        manaDrainPerSec = 1f;
    }
}

#endregion

#region Debuffs

public class ArmorBreakDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.ArmorBreak--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.ArmorBreak++;

        manaDrainPerSec = 1f;
    }
}

public class ManaSusceptibilityDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.ManaSusceptibility--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.ManaSusceptibility++;

        manaDrainPerSec = 1f;
    }
}

public class MartialIneptitiudeDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.MartialIneptitiude--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.MartialIneptitiude++;

        manaDrainPerSec = 1f;
    }
}

public class MagicalIneptitiudeDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.MagicalIneptitiude--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.MagicalIneptitiude++;

        manaDrainPerSec = 1f;
    }
}

public class CriticalVulnerabilityDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.CriticalVulnerability--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.CriticalVulnerability++;

        manaDrainPerSec = 1f;
    }
}

public class CriticalExploitDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.CriticalExploit--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.CriticalExploit++;

        manaDrainPerSec = 1f;
    }
}

public class StatusVulnerabilityDebuff : BuffORDebuffBase
{
    protected override void EndBuffDebuff()
    {
        targetHost.StatusVulnerability--;
    }

    protected override void SetupBuffDebuff()
    {
        targetHost.StatusVulnerability++;

        manaDrainPerSec = 1f;
    }
}

#endregion