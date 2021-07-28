using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    public GameActor actor;

    private const float MinValue = 0.8f;
    private const float MaxValue = 1.5f;
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
        EventCenter.Instance.EventTrigger(CameraEvent.ShakeCamera, actor, 2f, 0.2f);
    }

    /// <summary>
    /// 伤害结束
    /// </summary>
    public void EqDisable()
    {
        weaponData.state = WeaponData.STATE.IDLE;
    }
}