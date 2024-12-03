using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class Prey : MonoBehaviour
{
   public GameObject head;
   public GameObject body;
   public PreyState state;
   public Animator animator;
   public Collider collider;
   public AIPath AI;
   public Seeker seeker;
   public bool isFollowingPath;
   public bool isBeingAttacked;
   public int hp;
   public bool isDead;
   public HitState lastHitState;
   public Animal animal;
   public float maxSeeDistance = 25f;
   public float maxSeeAngle = 80f;
   public float idleTimer;
   public bool randomizePrey = true;

   [SerializeField] Animal hare;
   [SerializeField] Animal fox;
   [SerializeField] Animal deer;
   [SerializeField] Animal wolf;
   [SerializeField] Animal bear;

   [SerializeField] private PreyType _type;
   private void Awake()
   {
      UpdateAnimal();
      hp = animal.stats.hp;
      AI.maxSpeed = animal.stats.speed;
   }

   List<Transform> GetChildrenAnimalModels(GameObject gameObject)
   {
      List<Transform> children = new List<Transform>();

      for (int i = 0; i < gameObject.transform.childCount; i++) {
         var child = gameObject.transform.GetChild(i);

         if (child.name.Contains("Arm") == false)
         {
            children.Add(child);
         }
      }

      return children;
   }

   public void SetupPreyType(PreyType type)
   {
      _type = type;
      UpdateAnimal();
   }
   
   Animal ModelRootFromType(PreyType type)
   {
      switch (type)
      {
         case PreyType.Hare:
            return hare;
            break;
         case PreyType.Fox:
            return fox;
            break;
         case PreyType.Deer:
            return deer;
            break;
         case PreyType.Wolf:
            return wolf;
            break;
         case PreyType.Bear:
            return bear;
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }
   void ActivateRandomAnimalModel()
   {
      if (randomizePrey == false) return;
      var modelRoot = ModelRootFromType(_type);
      var models = GetChildrenAnimalModels(modelRoot.gameObject);
      var random = Random.Range(0, models.Count);
      models.ForEach(m => m.gameObject.SetActive(false));
      models[random].gameObject.SetActive(true);
      modelRoot.gameObject.SetActive(true);
   }

   void hideAllAnimalModels()
   {
      var hareModels = GetChildrenAnimalModels(hare.gameObject);
      hareModels.ForEach(m => m.gameObject.SetActive(false));
      hare.gameObject.SetActive(false);
      
      var foxModels = GetChildrenAnimalModels(fox.gameObject);
      foxModels.ForEach(m => m.gameObject.SetActive(false));
      fox.gameObject.SetActive(false);

      var deerModels = GetChildrenAnimalModels(deer.gameObject);
      deerModels.ForEach(m => m.gameObject.SetActive(false));
      deer.gameObject.SetActive(false);

      var wolfModels = GetChildrenAnimalModels(wolf.gameObject);
      wolfModels.ForEach(m => m.gameObject.SetActive(false));
      wolf.gameObject.SetActive(false);

      var bearModels = GetChildrenAnimalModels(bear.gameObject);
      bearModels.ForEach(m => m.gameObject.SetActive(false));
      bear.gameObject.SetActive(false);

   }

   void UpdateAnimal()
   {
      switch (_type)
      {
         case PreyType.Hare:
            animal = hare.GetComponent<Animal>();
            animator = hare.GetComponent<Animator>();
            head = hare.Head;
            body = hare.Body;
            break;
         case PreyType.Fox:
            animator = fox.GetComponent<Animator>();
            animal = fox.GetComponent<Animal>();
            head = fox.Head;
            body = fox.Body;
            break;
         case PreyType.Deer:
            animator = deer.GetComponent<Animator>();
            animal = deer.GetComponent<Animal>();
            head = deer.Head;
            body = deer.Body;
            break;
         case PreyType.Wolf:
            animator = wolf.GetComponent<Animator>();
            animal = wolf.GetComponent<Animal>();
            head = wolf.Head;
            body = wolf.Body;
            break;
         case PreyType.Bear:
            animator = bear.GetComponent<Animator>();
            animal = bear.GetComponent<Animal>();
            head = bear.Head;
            body = bear.Body;
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
      
      hideAllAnimalModels();
      ActivateRandomAnimalModel();
   }

   private void OnValidate()
   {
      UpdateAnimal();
   }
}

public enum PreyState
{
   Idle,
   Walk,
   Run,
   Die
}