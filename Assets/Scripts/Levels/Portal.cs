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

   private void OnTriggerStay(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         var distance = Vector3.Distance(other.gameObject.transform.position, this.transform.position);
         if (distance <= 2f)
         {
            GameManager.Instance.ShowWinScreen();
            gameObject.SetActive(false);
         }
      }
   }
}
