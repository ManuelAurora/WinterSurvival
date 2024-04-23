using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Вычисляем направление от этого объекта к камере
        Vector3 lookDirection = transform.position - Camera.main.transform.position;

        // Вычисляем поворот, необходимый для того, чтобы смотреть в противоположную сторону от камеры
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        // Применяем поворот к этому объекту
        transform.rotation = targetRotation;
    }
}
