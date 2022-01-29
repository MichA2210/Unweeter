using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBEnemy : TBCharacter
{
    private TBPlayer m_Player { get; set; }

    private Unit unit;
    public Animator animator;

    private const float k_AnimationProgressThreshold = 0.99f;

    // Event Listener
    public override void OnCombatChange(object sender, TBCSystemEventArgs e)
    {
        if (e.CombatEnd)
        {
            CombatSystem.ChangedCharacter -= OnCombatChange;
            return;
        }
        CharacterState = e.IsPlayer ? TBCharacterState.WaitingForOther : TBCharacterState.WaitingForSelf;
    }
    // MonoBehaviour
    void Start()
    {
        unit = GetComponent<Unit>();
    }
    void Update()
    {
        if( CharacterState == TBCharacterState.WaitingForSelf)
        {
            int r = Random.Range(0, 1);

            if (r == 1)
            {
                Heal(); 
            } 
            else
            {
                Heal();
            }
        }
        else if (CharacterState == TBCharacterState.Attacking)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                AfterAttack();
            }
        }
        else if (CharacterState == TBCharacterState.Healing)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                AfterHeal();
            }
        }
        else if (CharacterState == TBCharacterState.Dying)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                OnceDead();
            }
        }
    }
    // Setup
    public void Setup(TBCombat CombatSystem, TBPlayer Player)
    {
        base.Setup(CombatSystem);
        this.m_Player = Player;
        CharacterState = TBCharacterState.WaitingForOther;
    }
    public override void TakeDamage(int damage)
    {
        unit.currentHP -= damage;

        if (unit.currentHP <= 0)
        {
            unit.currentHP = 0;
            CombatSystem.ChangedCharacter -= OnCombatChange;
            animator.SetTrigger("Death");
            CharacterState = TBCharacterState.Dying;
        }
    }
    protected override void Attack()
    {
        CharacterState = TBCharacterState.Attacking;
        animator.SetTrigger("Attack");
    }
    protected override void Heal()
    {
        CharacterState = TBCharacterState.Healing;
        animator.SetTrigger("Heal");
    }
    protected override void AfterAttack()
    {
        m_Player.TakeDamage(unit.damage);
        CombatSystem.Next(this);
    }
    protected override void AfterHeal()
    {
        Debug.Log("Healing....");
        unit.currentHP += Random.Range(3, 15);
        unit.currentHP = unit.currentHP > unit.maxHP ? unit.maxHP : unit.currentHP;
        CombatSystem.Next(this);
    }
    protected override void OnceDead()
    {
        Destroy(transform.parent.gameObject);
        CombatSystem.End(this);
    }
    public float GetCurrentAnimatorTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.normalizedTime % 1;
        return currentTime;
    }
}
