using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Logic;

public class Arrow : MonoBehaviour
{
    public GameManager game;
    public Transform launchPoint;  
    public Transform target;
    public Prey prey;
    public float launchAngle = 45f; 
    public float targetRadius = 0.5f; 
    public float speedMultiplier = 2.0f;
    public float speed;

    public Rigidbody rb;         
    public  Vector3 targetPoint; 
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        transform.position = launchPoint.position;
        
        CalculateRandomTargetPoint(ref targetPoint, target, targetRadius);
        LaunchArrow(this);   
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        if (prey.lastHitState == HitState.Miss) { return;} 
        
        // Текущая позиция стрелы
        Vector3 currentPosition = transform.position;

        // Направление к цели
        Vector3 directionToTarget = targetPoint - currentPosition;

        // Коррекция скорости
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < 20)
        {
            rb.isKinematic = true;
            SimpleTranslation();
        }
       
    }

    void SimpleTranslation()
    {
        // Текущая позиция стрелы
        Vector3 currentPosition = transform.position;
        Vector3 directionToTarget = targetPoint - currentPosition;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        // Двигаем стрелу к цели
        float step = speed * Time.deltaTime; // Скорость движения стрелы
        transform.position = Vector3.MoveTowards(currentPosition, targetPoint, step);

        // Проверяем, достигла ли стрела цели
        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            transform.parent = target;
            rb.velocity = Vector3.zero;
            enabled = false; 
            game.DidHitPrey(prey);
        }
        
    }

}
