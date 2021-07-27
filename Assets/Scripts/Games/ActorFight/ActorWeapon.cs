using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorWeapon : MonoBehaviour
{
    public WeaponData weaponData;

    private const float MinValue = 0.8f;
    private const float MaxValue = 1.3f;
    public float GetAtk()
    {
        var randomDamage = (int)Random.Range(MinValue * weaponData.ATK, MaxValue * weaponData.ATK);
        return randomDamage;
    }

    /// <summary>
    /// 伤害开始
    /// </summary>
    public void EqEnable()
    {
        weaponData.state = WeaponData.STATE.ACTIVE;
        AudioMgr.Instance.PlaySound("atk1");
    }

    /// <summary>
    /// 伤害结束
    /// </summary>
    public void EqDisable()
    {
        weaponData.state = WeaponData.STATE.IDLE;
    }
}