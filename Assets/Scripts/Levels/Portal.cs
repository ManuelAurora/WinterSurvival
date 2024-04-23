using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Portal : MonoBehaviour
{
   private void Awake()
   {
      GetComponent<Rigidbody>().isKinematic = true;
      GetComponent<CapsuleCollider>().isTrigger = true;
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
      GameManager.Instance.Home();
      gameObject.SetActive(false);
      }
   }
}
