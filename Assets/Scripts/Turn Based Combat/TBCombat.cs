using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TBCharacterState
{
    Starting,
    WaitingForSelf, 
    WaitingForOther,
    Attacking,
    Healing,
    Moving,
    Dying
}

public class TBCSystemEventArgs : EventArgs
{
    public bool CombatEnd { get; set; }
    public bool IsPlayer { get; set; }
}

public class TBCombat
{
    public event EventHandler<TBCSystemEventArgs> ChangedCharacter;

    public TBCombat(TBPlayer player, TBEnemy enemy)
    {
        player.Setup(this, enemy, enemy.transform.parent.Find("PlayerBattlePos").transform.position);
        enemy.Setup(this, player);
    }

    public void Next(TBEnemy enemy)
    {
        OnChange(true);
    }

    public void Next(TBPlayer player)
    {
        OnChange(false);
    }

    public void End(TBEnemy enemy)
    {
        OnChange(true, true);
    }

    public void End(TBPlayer player)
    {
        OnChange(false, true);
    }

    private void OnChange(bool IsPlayer, bool end = false)
    {
        //thread safety copy
        EventHandler<TBCSystemEventArgs> handler = ChangedCharacter;
        Debug.Log("Event Change");
        handler?.Invoke(
                this,
                new TBCSystemEventArgs
                {
                    CombatEnd = end,
                    IsPlayer = IsPlayer
                }
            ) ;
    }
}