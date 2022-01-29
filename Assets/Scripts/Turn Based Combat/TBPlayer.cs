using UnityEngine;

public class TBPlayer : TBCharacter
{
    [SerializeField]
    private int m_SelectedOption = 0;
    private Vector2 m_MoveTo;

    private TBEnemy m_Enemy { get; set; }

    private const float k_PositioningThreshold = 0.0001f;
    private const float k_AnimationProgressThreshold = 0.99f;

    private Unit unit;
    private Animator animator;
    private CharacterController2D characterController2D;

    // Event Listener
    public override void OnCombatChange(object sender, TBCSystemEventArgs e)
    {
        if( e.CombatEnd)
        {
            CombatSystem.ChangedCharacter -= OnCombatChange;
            return;
        }
        CharacterState = e.IsPlayer ? TBCharacterState.WaitingForSelf : TBCharacterState.WaitingForOther;
    }
    // MonoBehaviour
    private void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        characterController2D = GetComponent<CharacterController2D>();
    }
    void Update()
    {
        if( CharacterState == TBCharacterState.Starting)
        {
            Vector2 vector = m_MoveTo - new Vector2(transform.position.x, transform.position.y);
            characterController2D.Move( vector.x * 0.5f, false, false);
            //string str = string.Format("m_MoveTo: {0}; transform.position: {1}", m_MoveTo, transform.position);
            //Debug.Log(str);
            if (vector.x < k_PositioningThreshold)
            {
                characterController2D.Face(true);
                CharacterState = TBCharacterState.WaitingForSelf;
            }
        }
        else if( CharacterState == TBCharacterState.WaitingForSelf)
        {
            if ( Input.GetKeyDown(KeyCode.A))
            {
                m_SelectedOption = (m_SelectedOption - 1) % 2;
            } else if ( Input.GetKeyDown(KeyCode.D))
            {
                m_SelectedOption = (m_SelectedOption + 1) % 2;
            }
            if ( Input.GetButtonUp("Select") )
            {
                switch (m_SelectedOption)
                {
                    case 0:
                        Attack();
                        break;
                    case 1:
                        Heal();
                        break;
                }
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
        base.Setup(CombatSystem);
        this.m_Enemy = Enemy;
        this.m_MoveTo = MoveTo;
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
        m_Enemy.TakeDamage(unit.damage);
        CombatSystem.Next(this);
    }
    protected override void AfterHeal()
    {
        unit.currentHP += Random.Range(3, 15);
        unit.currentHP = unit.currentHP > unit.maxHP ? unit.maxHP : unit.currentHP;
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
}
