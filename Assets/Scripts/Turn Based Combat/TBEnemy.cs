using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class TBEnemy : TBCharacter
{
    private TBPlayer m_Player { get; set; }
    private GameObject m_EnemyHealthBar { get; set; }

    private Unit unit;
    private Slider slider;
    public Animator animator;
    private AudioSource audioSource;

    // Event Listener
    public override void OnCombatChange(object sender, TBCSystemEventArgs e)
    {
        if (e.CombatEnd)
        {
           
            Debug.Log(SceneManager.GetActiveScene().buildIndex);
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                //cinematica
                StartCoroutine(TimeOut.Set(1f, () => SceneManager.LoadScene(5)));
            }
            CombatSystem.ChangedCharacter -= OnCombatChange;
            return;
        }
        CharacterState = e.IsPlayer ? TBCharacterState.WaitingForOther : TBCharacterState.WaitingForSelf;
    }
    // MonoBehaviour
    void Start()
    {
        unit = GetComponent<Unit>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (CharacterState == TBCharacterState.WaitingForSelf)
        {
            if (Random.Range(0f, 1f) > unit.attackHealBalance)
            {
                Attack();
            } else
            {
                Heal();
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
        slider.value = unit.currentHP = unit.maxHP;
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
            StartCoroutine(TimeOut.Set(1f, OnceDead));
        }
        else
        {
            CharacterState = TBCharacterState.Hurting;
        }
    }
    protected override void Attack()
    {
        CharacterState = TBCharacterState.Attacking;
        audioSource.Play();
        animator.SetTrigger("Attack");
        StartCoroutine(TimeOut.Set(1f, ()=>AfterAttack(unit.damage)) );
    }
    protected override void Heal()
    {
        CharacterState = TBCharacterState.Healing;
        animator.SetTrigger("Heal");
        int healing = Random.Range(unit.minHeal, unit.maxHeal+1);
        HealthAnimation(healing);
        StartCoroutine(TimeOut.Set(1f, () => AfterHeal(healing)));
    }
    protected void AfterAttack(int Damage)
    {
        m_Player.TakeDamage(unit.damage);
        CombatSystem.Next(this);
    }
    protected void AfterHeal(int healing)
    {
        unit.currentHP += healing;
        unit.currentHP = unit.currentHP > unit.maxHP ? unit.maxHP : unit.currentHP;
        CombatSystem.Next(this);
    }
    protected override void OnceDead()
    {
        Destroy(transform.parent.gameObject);
        CombatSystem.End(this);
    }

    //Animation
    void HealthAnimation(int healing)
    {
        StartCoroutine(
            TimeOut.InterpolateFloat(
                unit.currentHP,
                unit.currentHP + healing,
                .4f,
                x => slider.value = Mathf.Clamp(x, slider.minValue, slider.maxValue)
            )
        );
    }

    public void DamageAnimation(int damage)
    {
        StartCoroutine(
            TimeOut.InterpolateFloat(
                unit.currentHP,
                unit.currentHP - damage,
                .4f,
                x => slider.value = Mathf.Clamp(x, slider.minValue, slider.maxValue)
            )
        );
    }
    //Unimplemented
    protected override void AfterAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void AfterHeal()
    {
        throw new System.NotImplementedException();
    }
}
