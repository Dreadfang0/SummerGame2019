using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *Original Author: Saku Petlin
 * 26.6.2019
 */

public class PerkSystem : MonoBehaviour
{
    public PlayerController playerController;
    public int multishotUpgrade = 0;
    [SerializeField]
    private int healthUpgrade;
    [HideInInspector]
    public int healthLevel;
    [SerializeField]
    private int healthRestore;
    [SerializeField]
    private int damageUpgrade;
    [HideInInspector]
    public int damageLevel;
    [SerializeField]
    private float attackSpeedUpgrade = 0.1f;
    private float attackSpeed = 1;
    [HideInInspector]
    public int attackFrequencyLevel;
    [SerializeField]
    private int ammoMaxUpgrade;
    [SerializeField]
    private float reloadSpeedUpgrade;
    [HideInInspector]
    public int ammoReloadLevel;
    [SerializeField]
    private double lifeSteal = 0.02;
    [SerializeField]
    private float critChanceUpgrade = 5f;
    [SerializeField]
    private double critMultiUpgrade = 0.25;
    [HideInInspector]
    public int critMasteryLevel;
    public int burstUpgrade = 0;

    public bool FireProjectiles = false;
    public bool ExplosiveProjectiles = false;
    public bool PiercingProjectiles = false;
    public bool LifeSteal = false;
    public bool GlassCannon = false;
    public bool FrostProjectiles = false;
    public bool Berserk = false;

    [HideInInspector]
    public string multiShotText;
    [HideInInspector]
    public string fireProjectilesText;
    [HideInInspector]
    public string explosiveProjectilesText;
    [HideInInspector]
    public string piercingProjectilesText;
    [HideInInspector]
    public string attackSpeedText;
    [HideInInspector]
    public string damageText;
    [HideInInspector]
    public string healthUpgradeText;
    [HideInInspector]
    public string healthRestoreText;
    [HideInInspector]
    public string lifeStealText;
    [HideInInspector]
    public string ammoReloadSpeedText;
    [HideInInspector]
    public string criticalMasteryText;
    [HideInInspector]
    public string glassCannonText;
    [HideInInspector]
    public string burstText;
    [HideInInspector]
    public string frostProjectilesText;
    [HideInInspector]
    public string berserkText;

    [System.Serializable]
    public class PerkIcons
    {
        public Sprite multiShotIcon;
        public Sprite multiShotIcon2;
        public Sprite fireProjectilesIcon;
        public Sprite explosiveProjectilesIcon;
        public Sprite piercingProjectilesIcon;
        public Sprite attackSpeedIcon;
        public Sprite damageIcon;
        public Sprite healthUpgradeIcon;
        public Sprite healthRestoreIcon;
        public Sprite lifeStealIcon;
        public Sprite ammoReloadSpeedIcon;
        public Sprite criticalMasteryIcon;
        public Sprite glassCannonIcon;
        public Sprite burstIcon;
        public Sprite frostProjectilesIcon;
        public Sprite berserkIcon;
    }

    public PerkIcons perkIcons;

    public void UpdateTexts()
    {
        multiShotText = "Increases the number of projectiles you fire by 2";
        fireProjectilesText = "Your projectiles now set enemies on fire, dealing damage over time equal to your damage";
        explosiveProjectilesText = "Your projectiles now deal " + (100 * playerController.explosiveMultiplier) + "% of your damage in small area around them";
        piercingProjectilesText = "Your projectiles now pierce enemies";
        attackSpeedText = "Increases your attack speed by " + (attackSpeedUpgrade * 100) + "%";
        if(GlassCannon == true)
        {
            damageText = "Increases your damage by " + damageUpgrade * 2;
            berserkText = "You deal " + playerController.berserkDamage * 2 + " bonus damage for every " + playerController.berserkThreshold * 100 + "% health you are missing";
        }

        else
        {
            damageText = "Increases your damage by " + damageUpgrade;
            berserkText = "You deal " + playerController.berserkDamage + " bonus damage for every " + playerController.berserkThreshold * 100 + "% health you are missing";
        }

        healthUpgradeText = "Increases your maximum and current health by " + healthUpgrade;
        healthRestoreText = "Restores your current health by " + healthRestore;
        lifeStealText = "Killing enemies restore your health by " + (lifeSteal * 100) + "%";
        ammoReloadSpeedText = "Max ammo + " + ammoMaxUpgrade + " and reload speed - " + ((1 - reloadSpeedUpgrade) * 100) + "%";
        criticalMasteryText = "Critical hit chance + " + critChanceUpgrade + "% and critical damage multiplier + " + critMultiUpgrade;
        glassCannonText = "Doubles your damage and halves your health";
        burstText = "Adds another projectile after every projectile you shoot";
        frostProjectilesText = "Your projectiles now slow enemies movement speed by " + (playerController.slowMultiplier * 100) + "%";
    }

    public void LifeStealHeal()
    {
            playerController.health = Mathf.RoundToInt((float)(playerController.health + (playerController.healthMax * lifeSteal)));
    }



    public void MultishotPerk()
    {
        multishotUpgrade += 1;
        if (multishotUpgrade != 2)
            GameManager.instance.MenuSkillCount[0].text = multishotUpgrade.ToString();
        else
            GameManager.instance.MenuSkillCount[0].text = "MAXED";
    }

    public void FireProjectilesPerk()
    {
        FireProjectiles = true;
        GameManager.instance.MenuSkillCount[1].text = "MAXED";
    }

    public void ExplosiveProjectilesPerk()
    {
        ExplosiveProjectiles = true;
        GameManager.instance.MenuSkillCount[2].text = "MAXED";
    }

    public void PiercingProjectilesPerk()
    {
        PiercingProjectiles = true;
        GameManager.instance.MenuSkillCount[3].text = "MAXED";
    }

    public void AttackSpeedPerk()
    {
        attackFrequencyLevel++;
        attackSpeed += attackSpeedUpgrade;
        playerController.attackFrequency = playerController.baseAttackFrequency;
        playerController.attackFrequency /= attackSpeed;
        GameManager.instance.MenuSkillCount[4].text = attackFrequencyLevel.ToString();
    }

    public void DamagePerk()
    {
        damageLevel++;
        GameManager.instance.MenuSkillCount[5].text = damageLevel.ToString();

        if(GlassCannon == true)
        {
            playerController.baseDamage += damageUpgrade * 2;
            playerController.currentDamage += damageUpgrade * 2;
        }

        else
        {
            playerController.baseDamage += damageUpgrade;
            playerController.currentDamage += damageUpgrade;
        }


    }

    /*public void HealthUpgradePerk()
    {
        healthLevel++;
        playerController.healthMax += healthUpgrade;
        playerController.health += healthUpgrade;
        GameManager.instance.MenuSkillCount[6].text = healthLevel.ToString();
    }*/

    /*public void HealthRestorePerk()
    {
        playerController.health += healthRestore;
    }*/

    public void LifeStealPerk()
    {
        LifeSteal = true;
        GameManager.instance.MenuSkillCount[7].text = "MAXED";
    }

    public void AmmoReloadSpeedPerk()
    {
        ammoReloadLevel++;
        playerController.reloadSpeed *= reloadSpeedUpgrade;
        playerController.ammoMax += ammoMaxUpgrade;
        GameManager.instance.MenuSkillCount[8].text = ammoReloadLevel.ToString();
        playerController.ammo = playerController.ammoMax;
    }

    public void CriticalMasteryPerk()
    {
        critMasteryLevel++;
        playerController.critChance += critChanceUpgrade;
        playerController.critMultiplier += critMultiUpgrade;
        GameManager.instance.MenuSkillCount[6].text = critMasteryLevel.ToString();
    }

    public void GlassCannonPerk()
    {
        GlassCannon = true;
        playerController.baseDamage *= 2;
        playerController.currentDamage *= 2;
        playerController.healthMax /= 2;
        GameManager.instance.MenuSkillCount[9].text = "MAXED";
    }

    public void BurstPerk()
    {
        burstUpgrade += 1;
        if(burstUpgrade != 2)
        {
            GameManager.instance.MenuSkillCount[10].text = burstUpgrade.ToString();
        }

        else
        {
            GameManager.instance.MenuSkillCount[10].text = "MAXED";
        }
    }

    public void FrostProjectilesPerk()
    {
        FrostProjectiles = true;
        GameManager.instance.MenuSkillCount[11].text = "MAXED";
    }

    public void BerserkPerk()
    {
        Berserk = true;
        GameManager.instance.MenuSkillCount[12].text = "MAXED";
    }
}
