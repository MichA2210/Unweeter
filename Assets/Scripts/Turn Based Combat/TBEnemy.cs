using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TBEnemy : TBCharacter
{
    private TBPlayer m_Player { get; set; }
    private GameObject m_EnemyHealthBar { get; set; }

    private Unit unit;
    private Slider slider;
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
                Attack(); 
            }
            else
            {
                Attack();
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
    public void Setup(TBCombat CombatSystem, TBPlayer Player, GameObject EnemyHealthBar)
    {
        Setup(CombatSystem);
        m_Player = Player;
        m_EnemyHealthBar = EnemyHealthBar;
        m_EnemyHealthBar.SetActive(true);
        slider = m_EnemyHealthBar.GetComponent<Slider>();
        CharacterState = TBCharacterState.WaitingForOther;

        slider.minValue = 0;
        slider.maxValue = unit.maxHP;
        slider.value    = unit.currentHP = unit.maxHP;
    }
    public override void TakeDamage(int damage)
    {
        unit.currentHP -= damage;

        slider.value = unit.currentHP;

        if (unit.currentHP <= 0)
        {
            unit.currentHP = 0;
            CombatSystem.ChangedCharacter -= OnCombatChange;
            animator.SetTrigger("Death");
            CharacterState = TBCharacterState.Dying;
        }
        else
        {
            CharacterState = TBCharacterState.Hurting;
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
        unit.currentHP += Random.Range(3, 15);
        unit.currentHP = unit.currentHP > unit.maxHP ? unit.maxHP : unit.currentHP;
        slider.value = unit.currentHP;
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
