using UnityEngine;

public interface ITBCharacter
{
    void OnCombatChange(object sender, TBCSystemEventArgs e);
    void TakeDamage(int damage);
}

// Heredado a todos los personajes que lo usan
public abstract class TBCharacter : MonoBehaviour, ITBCharacter
{
    public TBCombat CombatSystem;
    public TBCharacterState CharacterState;

    public void Setup(TBCombat CombatSystem)
    {
        this.CombatSystem = CombatSystem;
        this.CombatSystem.ChangedCharacter += OnCombatChange;

        CharacterState = TBCharacterState.Starting;
    }

    public abstract void OnCombatChange(object sender, TBCSystemEventArgs e);
    public abstract void TakeDamage(int damage);
    protected abstract void Attack();
    protected abstract void AfterAttack();
    protected abstract void Heal();
    protected abstract void AfterHeal();
    protected abstract void OnceDead();
}
