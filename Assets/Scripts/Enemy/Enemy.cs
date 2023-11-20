using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health Settings")] 
    [SerializeField] protected int Health;

    [Header("Move Settings")] 
    [SerializeField] protected float MoveSpeed;
    [SerializeField] protected float StopDistance;

    [Header("Drop Settings")] 
    [Range(0, 1)] 
    [SerializeField] protected float DropRate;
    [SerializeField] protected GameObject[] ItemsToDrop;
    
    [Header("Elements")] 
    [SerializeField] protected Animator EnemyAnimator;
    [SerializeField] protected ParticleSystem DamageEffect;
    [SerializeField] protected GameObject XPCollectablePrefab;

    [Header("Sounds Effects")] 
    [SerializeField] private AudioClip[] DeathClips;

    private CapsuleCollider capsuleCollider;
    private AudioSource audioSource;
    
    private Transform _player;
    private bool _canMove = true;
    private bool _canAttack = true;
    private bool _isDead;
    private float _attackCooldown;
    protected int _currentHealth;

    public static Action<Enemy> OnDead;
    public Action<int> OnTakeDamage;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        _currentHealth = Health;
    }

    private void OnEnable()
    {
        PlayerController.OnDie += PlayerController_OnDie;
    }

    private void OnDisable()
    {
        PlayerController.OnDie -= PlayerController_OnDie;
    }

    void Update()
    {
        if (!_canAttack) return;
        
        _attackCooldown -= Time.deltaTime;

        if (!_canMove) return;
        
        Vector3 direction = (_player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        
        if (Vector3.Distance(transform.position, _player.position) > StopDistance)
        {
            EnemyAnimator.SetBool("isMoving", true);
            transform.position = Vector3.MoveTowards(transform.position, _player.position, MoveSpeed * Time.deltaTime);
        }
        else
        {
            EnemyAnimator.SetBool("isMoving", false);
            
            if (_attackCooldown <= 0)
            {
                _attackCooldown = 3;
                EnemyAnimator.SetTrigger("attack");
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (_isDead) return;
        
        OnTakeDamage?.Invoke(damage);

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            StartCoroutine(Die());
            return;
        }
        
        DamageEffect.Play();
    }
    
    private IEnumerator Die()
    {
        _canMove = false;
        _isDead = true;
        capsuleCollider.isTrigger = true;

        EnemyAnimator.SetTrigger("dead");
        audioSource.PlayOneShot(DeathClips[Random.Range(0, DeathClips.Length)]);
        
        TryDropXP();
        TryDropItem();

        OnDead?.Invoke(this);

        yield return new WaitForSeconds(2f);

        BeforeDestroying();
        Destroy(gameObject);
    }

    protected virtual void BeforeDestroying()
    {
        SaveManager.Instance.AddKill();
    }

    protected virtual void TryDropXP()
    {
        Instantiate(XPCollectablePrefab, transform.position, Quaternion.identity);
    }

    private void TryDropItem()
    {
        if (ItemsToDrop.Length == 0) return;
        
        float rate = Random.Range(0, 101);

        if (rate <= DropRate * 100)
        {
            Vector3 spawnPosition = transform.position;
            GameObject dropItem = ItemsToDrop[Random.Range(0, ItemsToDrop.Length)];
            
            spawnPosition.y = dropItem.transform.position.y;
            
            Instantiate(dropItem, spawnPosition, Quaternion.identity);
        }
    }

    public void Setup(Transform player)
    {
        _player = player;
    }

    private void PlayerController_OnDie()
    {
        _canAttack = false;
        _canMove = false;
        EnemyAnimator.SetBool("isMoving", false);
    }
}
