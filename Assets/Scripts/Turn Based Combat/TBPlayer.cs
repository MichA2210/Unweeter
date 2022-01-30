using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TBPlayer : TBCharacter
{
    [SerializeField]
    private Vector2 m_MoveTo;

    private TBEnemy m_Enemy { get; set; }

    private bool m_hasLanded = false;

    [SerializeField]
    [Range(0.1f, 5f)]
    private float k_DisplacementSpeed = 2.5f;

    [SerializeField]
    [Range(0.01f, 0.5f)]
    private float k_PositioningThreshold = 0.08f;
    private const float k_AnimationProgressThreshold = 0.99f;

    private GameObject combatCanvas;
    public  GameObject enemyHealthBar;
    private Button attackButton;
    private Slider healthSlider;
    private Unit unit;
    private Animator animator;
    private CharacterController2D characterController2D;

    // Event Listener
    public override void OnCombatChange(object sender, TBCSystemEventArgs e)
    {
        if( e.CombatEnd )
        {
            combatCanvas.SetActive(false);
            enemyHealthBar.SetActive(false);
            CombatSystem.ChangedCharacter -= OnCombatChange;
            EventSystem.current.SetSelectedGameObject(null);
            GetComponent<PlayerMovement>().SetMove(true);
            return;
        }
        CharacterState = e.IsPlayer ? TBCharacterState.WaitingForSelf : TBCharacterState.WaitingForOther;
    }
    // MonoBehaviour
    private void Start()
    {
        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
        combatCanvas = GameObject.Find("CombatCanvas");
        enemyHealthBar = GameObject.Find("EnemyHealthBar");
        combatCanvas.SetActive(false);
        enemyHealthBar.SetActive(false);
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        characterController2D = GetComponent<CharacterController2D>();

        healthSlider.minValue = 0;
        healthSlider.maxValue = unit.maxHP;
        healthSlider.value = unit.maxHP;
    }
    void Update()
    {
        if(CombatSystem != null)
        if( CharacterState == TBCharacterState.Starting)
        {
            attackButton.Select();
            float deltaX = m_MoveTo.x - transform.position.x;
            characterController2D.Move( k_DisplacementSpeed * deltaX / Mathf.Abs(deltaX), false, false);
            if ( Mathf.Abs(deltaX) < k_PositioningThreshold && m_hasLanded )
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                characterController2D.Face(true);
                CharacterState = TBCharacterState.WaitingForSelf;
                combatCanvas.SetActive(true);
            }
        }
        else if ( CharacterState == TBCharacterState.Attacking)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                AfterAttack();
            }
        }
        else if ( CharacterState == TBCharacterState.Healing)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                AfterHeal();
            }
        }
        else if ( CharacterState == TBCharacterState.Hurting)
        {
            // Check if attack animation ended
            if (GetCurrentAnimatorTime(animator) > k_AnimationProgressThreshold)
            {
                CharacterState = TBCharacterState.WaitingForSelf;
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
    public void Setup(TBCombat CombatSystem, TBEnemy Enemy, Vector2 MoveTo) {
        Setup(CombatSystem);
        m_Enemy = Enemy;
        m_MoveTo = MoveTo;
        m_hasLanded = false;
        //m_hasLanded = GetComponent<CharacterController2D>().Grounded;
    }
    // TBCharacter implementation
    public override void TakeDamage(int damage)
    {
        unit.currentHP -= damage;
        if ( unit.currentHP <= 0)
        {
            unit.currentHP = 0;
            animator.SetTrigger("Death");
            CharacterState = TBCharacterState.Dying;
        }else
        {
            CharacterState = TBCharacterState.Hurting;
        }
        healthSlider.value = unit.currentHP;
    }
    public void ReceiveHeal(int healing)
    {
        unit.currentHP += healing;

        if (unit.currentHP >= unit.maxHP)
        {
            unit.currentHP = unit.maxHP;
        }
        //Heal();
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
        m_Enemy.TakeDamage(unit.damage);
        CombatSystem.Next(this);
    }
    protected override void AfterHeal()
    {
        unit.currentHP += Random.Range(3, 15);
        unit.currentHP = unit.currentHP > unit.maxHP ? unit.maxHP : unit.currentHP;
        healthSlider.value = unit.currentHP;
        CombatSystem.Next(this);
    }
    protected override void OnceDead()
    {
        CombatSystem.End(this);
        //Game Over Screen
    }
    public float GetCurrentAnimatorTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.normalizedTime % 1;
        return currentTime;
    }

    // Button Events
    public void OnAttackButton()
    {
        if (CharacterState == TBCharacterState.WaitingForSelf)
        {
            Attack();
        }
    }
    public void OnHealButton()
    {
        if (CharacterState == TBCharacterState.WaitingForSelf)
        {
            Heal();
        }
    }
    public void OnGrounded()
    {
        m_hasLanded = true;
    }
}
