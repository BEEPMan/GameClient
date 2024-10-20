using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _id;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxhp;
    [SerializeField]
    protected int _mp;
    [SerializeField]
    protected int _maxmp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _magicAttack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected int _magicDefense;
    [SerializeField]
    protected float _moveSpeed;

    public int ID { get { return _id; } set { _id = value; } }
    public int HP { get { return _hp; } set { _hp = value; } }
    public int MaxHP { get { return _maxhp; } set { _maxhp = value; } }
    public int MP { get { return _mp; } set { _mp = value; } }
    public int MaxMP { get { return _maxmp; } set { _maxmp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int MagicAttack { get { return _magicAttack; } set { _magicAttack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public int MagicDefense { get { return _magicDefense; } set { _magicDefense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    void Start()
    {
        _hp = 100;
        _maxhp = 100;
        _mp = 100;
        _maxmp = 100;
        _attack = 10;
        _magicAttack = 0;
        _defense = 5;
        _magicDefense = 5;
        _moveSpeed = 5.0f;
    }

    public virtual void OnAttacked(Stat attacker)
    {
        int damage = attacker.Attack * (100 / (100 + Defense));
        HP -= damage;
        if (HP <= 0)
        {
            Debug.Log("Die");
            OnDead(attacker);
        }
        Debug.Log("Hit");
    }

    public virtual void OnMagicAttacked(Stat attacker)
    {
        int damage = attacker.Attack * (100 / (100 + MagicDefense));
        HP -= damage;
        if (HP <= 0)
        {
            Debug.Log("Die");
            OnDead(attacker);
        }
    }

    protected virtual void OnDead(Stat attacker)
    {
        Destroy(gameObject);
    }
}
