using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private int setHealth;
    [SerializeField]
    private Stat health;
    [SerializeField]
    private float movementSpeed = 2f;
    [SerializeField]
    private int damageByMonster;
    [SerializeField]
    private int reward;
    private SpriteRenderer spriteRenderer;
    public bool Alive { get => health.CurrentValue > 0; }
    protected Animator myAnimator;
    private Vector3 spawnPoint;
    private Vector3 friendlyBase;
    private Vector3 targetPoint;

    public bool IsActive{ get; set; }

    private readonly List<int> correctIndexesOfWaypoints = new List<int>() { 4, 5, 0, 1, 6, 7, 2, 3, 8, 9 };
    private int index = 0;
    private List<TileScript> wayPoints;


    
    public void Spawn()
    {
        wayPoints = LevelManager.Instance.GetWaypoints();
        myAnimator = GetComponent<Animator>();
        this.health.MaxValue = setHealth;
        this.health.CurrentValue = this.health.MaxValue;

        targetPoint = wayPoints[correctIndexesOfWaypoints[0]].WorldPosition;

        transform.position = LevelManager.Instance.Tiles[new Point(1, 2)].WorldPosition;
        IsActive = true;
    }
    private void Move()
    {
        if (IsActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
            {
                if (index < wayPoints.Count - 1)
                {
                    index++;
                    targetPoint = wayPoints[correctIndexesOfWaypoints[index]].WorldPosition;
                } 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("friend"))
        {
            Release();
            GameManager.Instance.Lives--;
        }
        if (collision.CompareTag("Tile"))
        {
            spriteRenderer.sortingOrder = collision.GetComponent<TileScript>().GridPosition.Y;
        }
    }
    public void Release()
    {
        index = 0;
        IsActive = false; 
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveMonster(this);
    }

    private void Update()
    {
        Move();
    }
    private void Awake()
    {
        health.Initialize();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(int damage)
    {
        if (IsActive)
        {
            SoundManager.Instance.PlaySfx("getHitted");
            health.CurrentValue -= damage;
            if (health.CurrentValue <= 0)
            {
                SoundManager.Instance.PlaySfx("deathSound");
                GameManager.Instance.Currency += reward;
                myAnimator.SetTrigger("Die");
                IsActive = false;
                GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
    }
}
