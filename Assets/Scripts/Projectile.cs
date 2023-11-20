using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;
    private int _damage;

    private void Start()
    {
        Destroy(gameObject, 1);
    }

    private void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    public void Setup(float speed, Vector3 direction, int damage)
    {
        _speed = speed;
        _direction = direction;
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            Enemy enemy = other.transform.GetComponent<Enemy>();
            enemy.TakeDamage(_damage);
        }
    }
}
