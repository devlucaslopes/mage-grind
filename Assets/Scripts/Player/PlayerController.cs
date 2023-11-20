using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")] 
    [SerializeField] private PlayerAttributes BaseAttributes;
    
    [Header("Health Settings")]
    [SerializeField] private ParticleSystem DamageEffect;

    [Header("Shoot Settings")]
    [SerializeField] private GameObject ProjectilePrefab;

    [Header("Sound Effects")] 
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip TakeDamageClip;
    [SerializeField] private AudioClip GemsClip;
    [SerializeField] private AudioClip HealClip;
    [SerializeField] private AudioClip DeathClip;

    [Header("Footstep SFX")] 
    [SerializeField] private AudioSource FootstepAudioSource;
    
    [Header("References")] 
    [SerializeField] private Transform ForwardPoint;
    [SerializeField] private Transform ShootPoint;
    [SerializeField] private Animator Animator;

    private CharacterController characterController;
    private Camera mainCamera;
    private AudioSource audioSource;
    
    private bool _canMove = true;
    private float _rotation;
    private bool _isMoving;
    private bool _isAttacking;
    private bool _isRecovering;
    private float _attackCooldown;
    private float _recoveryCooldown;
    private int _totalHealth;
    private int _currentHealth;
    private int _currentLevel = 1;
    private int _currentXP;
    private int _damage;
    private float _moveSpeed;
    private float _projectileSpeed;
    private float _timeToRecovery;
    private Vector3 _lookTo;
    private bool _isDead;
    private Vector2 _input;

    public static Action<int> OnTakeDamage;
    public static Action OnDie;
    public static Action<int> OnLevelUp;
    public static Action OnGemsCollected;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        _totalHealth = BaseAttributes.GetHealth();
        _currentHealth = BaseAttributes.GetHealth();
        _damage = BaseAttributes.GetDamage();
        _moveSpeed = BaseAttributes.GetMoveSpeed();
        _projectileSpeed = BaseAttributes.GetProjectileSpeed();
        _timeToRecovery = BaseAttributes.GetTimeToRecovery();
        
        mainCamera = Camera.main;
    }

    private void Start()
    {
        UIManager.Instance.UpdatePlayerHealthBar(_totalHealth, _currentHealth);
    }

    private void OnEnable()
    {
        XPCollector.OnXPCollected += XPCollector_OnXPCollected;
        UIManager.OnStatsUpgraded += UIManager_OnStatsUpgraded;
    }

    private void OnDisable()
    {
        XPCollector.OnXPCollected -= XPCollector_OnXPCollected;
        UIManager.OnStatsUpgraded -= UIManager_OnStatsUpgraded;
    }

    void Update()
    {
        if (_isRecovering)
        {
            _recoveryCooldown -= Time.deltaTime;

            if (_recoveryCooldown <= 0) _isRecovering = false;
        }
        
        Rotate();
        Move();
    }

    private void Move()
    {
        if (!_canMove) return;

        Vector3 direction = new Vector3(_input.x, 0, _input.y);
        characterController.Move(direction * (_moveSpeed * Time.deltaTime));

        if (_input != Vector2.zero)
        {
            if (!FootstepAudioSource.enabled) FootstepAudioSource.enabled = true;
            
            Animator.SetBool("isMoving", true);
        }
        else
        {
            if (FootstepAudioSource.enabled) FootstepAudioSource.enabled = false;
            
            Animator.SetBool("isMoving", false);
        }

        
    }

    private void Rotate()
    {
        if (GameManager.Instance.IsPaused()) return;
        
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.value);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _lookTo = hit.point;
            _lookTo.y = 0;
        }
        
        transform.LookAt(_lookTo);
    }
    
    private void Attack()
    {
        _isAttacking = true;
        _canMove = false;
        
        Animator.SetTrigger("attack1");
    }

    public void ShootProjectile()
    {
        _canMove = true;
        _isAttacking = false;
        
        Vector3 direction = ForwardPoint.position - transform.position;
        direction.y = 0;
        
        Projectile projectile = Instantiate(ProjectilePrefab, ShootPoint.position, transform.rotation).GetComponent<Projectile>();
        projectile.Setup(_projectileSpeed, direction, _damage);
        
        audioSource.PlayOneShot(AttackClip);
    }

    private void TakeDamage(int damage)
    {
        if (_isRecovering || _isDead) return;

        _currentHealth -= damage;
        
        DamageEffect.Play();
        audioSource.PlayOneShot(TakeDamageClip);
        
        OnTakeDamage?.Invoke(damage);
        UIManager.Instance.UpdatePlayerHealthBar(_totalHealth, _currentHealth);
        
        if (_currentHealth <= 0)
        {
            _isDead = true;
            SaveManager.Instance.AddLose();
            Animator.SetTrigger("die");
            audioSource.PlayOneShot(DeathClip);
            OnDie?.Invoke();
            return;
        }
        
        _isRecovering = true;
        _isAttacking = false;
        _recoveryCooldown = _timeToRecovery;
    }

    public void SetCanMove(bool value)
    {
        _canMove = value;
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (_isAttacking) return;
        if (value.performed) Attack();
    }
    
    public void OnMove(InputAction.CallbackContext value)
    {
        _input = value.ReadValue<Vector2>();
    }

    private void XPCollector_OnXPCollected(int xp)
    {
        _currentXP += xp;
        int totalXP = _currentLevel * 10;

        if (_currentXP >= totalXP)
        {
            _currentXP = 0;
            _currentLevel++;
            
            OnLevelUp?.Invoke(_currentLevel);
        }
        
        // TODO: Retorar isso para evento
        UIManager.Instance.UpdateExperienceBar(totalXP, _currentXP);
    }

    private void UIManager_OnStatsUpgraded(PlayerAttributes.PlayerStats stats)
    {
        switch (stats)
        {
            case PlayerAttributes.PlayerStats.Attack:
                _damage++;
                break;
            case PlayerAttributes.PlayerStats.Health:
                _totalHealth += 10;
                break;
            case PlayerAttributes.PlayerStats.MoveSpeed:
                _moveSpeed += _moveSpeed * 0.2f;
                break;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "Enemy Weapon": 
                TakeDamage(1);
                break;
            case "Heal":
                int healAmount = Mathf.RoundToInt(_totalHealth * 0.25f);
                _currentHealth += healAmount;
            
                if (_currentHealth >= _totalHealth) _currentHealth = _totalHealth;
            
                UIManager.Instance.UpdatePlayerHealthBar(_totalHealth, _currentHealth);
                audioSource.PlayOneShot(HealClip);
            
                Destroy(other.gameObject);
                break;
            case "Gems":
                SaveManager.Instance.AddGems(1);
                OnGemsCollected?.Invoke();
                audioSource.PlayOneShot(GemsClip);
                Destroy(other.gameObject);
                break;
        }
    }
}
