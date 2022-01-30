using UnityEngine;
public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int minHeal;
    public int maxHeal;
    [Range(0f, 1f)]
    public float attackHealBalance;
}
