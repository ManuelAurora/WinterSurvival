using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
using Pathfinding;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public static class Logic
{
    // Player

    public static void SimulatePlayer(
        Player player,
        GameUI ui,
        GameState_SO gameState,
        GameManager game,
        CinemachineTargetGroup huntCameraGroupTarget,
        CinemachineVirtualCamera camera,
        CinemachineVirtualCamera huntCamera
    )
    {
        Tree GetNearestTree(List<Tree> treesAround, Vector3 playerPosition)
        {
            if (treesAround == null || treesAround.Count == 0)
                return null;

            // Assuming treesAround is a list of Tree objects and each Tree has a Transform
            Tree nearestTree = treesAround.OrderBy(tree => Vector3.Distance(tree.transform.position, playerPosition))
                .FirstOrDefault();
            return nearestTree;
        }

        bool isActive = UltimateJoystick.GetJoystickState("Main");

        if (gameState.current == GameState.Wood)
        {
            var tree = GetNearestTree(player.treesAround, player.transform.position);

            if (player.cutCoroutine == null && isActive == false && tree != null)
            {
                player.nearestTree = tree;
                player.cutCoroutine = player.StartCoroutine(CutTree(player, tree, game, gameState));
                player.state = PlayerState.Cutting;
            }
        }

        switch (player.state)
        {
            case PlayerState.Hunting:
                UltimateJoystick.DisableJoystick("Main");

                ui.HuntBase.SetActive(player.didShot == false);

                player.animator.SetBool("Run", false);
                player.animator.SetBool("Aim", true);

                player.transform.LookAt(player.CurrentPrey.transform.position);

                if (player.isAimingAtarget == false)
                {
                    player.isAimingAtarget = true;

                    HuntArrowStartMoving(ui);

                    var playerTarget = new CinemachineTargetGroup.Target
                    {
                        radius = 2,
                        target = player.transform,
                        weight = 1f
                    };

                    var preyTarget = new CinemachineTargetGroup.Target
                    {
                        radius = 2,
                        target = player.CurrentPrey.transform,
                        weight = 1f
                    };

                    huntCameraGroupTarget.m_Targets = new[] { playerTarget, preyTarget };

                    camera.Priority = 1;
                    huntCamera.Priority = 100;
                }

                break;
            case PlayerState.Cutting:
                player.animator.SetBool("Run", false);
                var directionToTree = player.nearestTree.GetComponent<Renderer>().bounds.center;
                directionToTree.y = player.transform.position.y;
                player.transform.LookAt(directionToTree);

                if (player.cutCoroutine == null)
                {
                    player.animator.SetBool("isChopping", false);
                    player.state = PlayerState.Idle;
                    player.StopAllCoroutines();
                    player.cutCoroutine = null;
                    player.progressParent.gameObject.SetActive(false);
                }

                if (isActive)
                {
                    player.animator.SetBool("isChopping", false);
                    player.nearestTree = null;
                    player.state = PlayerState.Running;
                    player.StopAllCoroutines();
                    player.cutCoroutine = null;
                    player.progressParent.gameObject.SetActive(false);
                }

                break;
            case PlayerState.Idle:
                huntCamera.Priority = 1;
                camera.Priority = 100;
                player.isAimingAtarget = false;
                ui.HuntBase.SetActive(false);
                UltimateJoystick.EnableJoystick("Main");
                player.animator.SetBool("Run", false);

                if (isActive)
                {
                    float h = UltimateJoystick.GetHorizontalAxis("Main");
                    float v = UltimateJoystick.GetVerticalAxis("Main");
                    Vector3 direction = new Vector3(h, 0, v);

                    if (direction.magnitude > 0.1f)
                    {
                        player.state = PlayerState.Running;
                        player.StopAllCoroutines();
                        player.cutCoroutine = null;
                    }
                }

                break;
            case PlayerState.Running:
                if (!isActive)
                {
                    player.state = PlayerState.Idle;
                }

                player.transform.DOKill(true);
                player.isAimingAtarget = false;
                SimulatePlayerMovement(
                    player.transform,
                    player.controller,
                    gameState.current == GameState.Hunt ? player.huntTurnSpeed : player.chopTurnSpeed,
                    player.speed
                );
                player.animator.SetBool("Run", true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void SimulatePlayerMovement(
        Transform player,
        CharacterController controller,
        float rotationSpeed,
        float speed
    )
    {
        float h = UltimateJoystick.GetHorizontalAxis("Main");
        float v = UltimateJoystick.GetVerticalAxis("Main");
        Vector3 direction = new Vector3(h, 0, v);

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();
            Vector3 movement = direction * (speed * Time.deltaTime);

            controller.Move(movement);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation =
                Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        controller.Move(new Vector3(0, -0.1f, 0));
    }

    public static void SimulatePrey(Prey prey, AIPath ai, Seeker seeker, Player player)
    {
        Vector3 directionToPlayer = player.transform.position - prey.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(prey.transform.forward, directionToPlayer);

        if ((distanceToPlayer <= 30 && angleToPlayer <= 60) || distanceToPlayer <= 10)
        {
            prey.state = PreyState.Run;
        }
        else
        {
            // Debug.Log("Player is not visible.");
        }

        switch (prey.state)
        {
            case PreyState.Idle:
                prey.collider.enabled = true;
                prey.animator.SetBool("Walk", false);
                prey.animator.SetBool("Run", false);

                if (prey.idleTimer == 0)
                {
                    var random = Random.Range(0, 2);
                    prey.state = random == 0 ? PreyState.Walk : PreyState.Idle;
                
                    if (prey.state == PreyState.Idle)
                    {
                        prey.idleTimer = Random.Range(0f, 5f);
                    }
                }

                break;
            case PreyState.Walk:
                prey.AI.maxSpeed = prey.animal.stats.speed;
                prey.collider.enabled = true;
                if (prey.isFollowingPath == false)
                {
                    prey.isFollowingPath = true;
                
                    var path = RandomPath.Construct(prey.transform.position, 10000);
                    // path.spread = 2000;
                
                    seeker.StartPath(path, path =>
                    {
                        ai.maxSpeed = 3f;
                        ai.SetPath(path);
                        prey.animator.SetBool("Run", false);
                        prey.animator.SetBool("Walk", true);
                    });
                }
                else
                {
                    if (ai.reachedEndOfPath)
                    {
                        prey.isFollowingPath = false;
                        var random = Random.Range(0, 2);
                        prey.state = random == 0 ? PreyState.Walk : PreyState.Idle;
                
                        if (prey.state == PreyState.Idle)
                        {
                            prey.idleTimer = Random.Range(0f, 10f);
                        }
                    }
                }

                break;
            case PreyState.Run:
                prey.collider.enabled = false;
                prey.AI.maxSpeed = prey.animal.stats.runSpeed;

                if (prey.isFollowingPath == false)
                {
                    prey.isFollowingPath = true;

                    var path = FleePath.Construct(prey.transform.position, player.transform.position, 30000);
                    path.spread = 5000;
                    path.aimStrength = 1;

                    seeker.StartPath(path, path =>
                    {
                        ai.maxSpeed = 12;
                        ai.SetPath(path);
                        prey.animator.SetBool("Run", true);
                        prey.animator.SetBool("Walk", false);
                    });
                }
                else
                {
                    if (ai.reachedEndOfPath)
                    {
                        prey.isBeingAttacked = false;
                        prey.isFollowingPath = false;
                        var random = Random.Range(0, 2);
                        prey.state = random == 0 ? PreyState.Walk : PreyState.Idle;

                        if (prey.state == PreyState.Idle)
                        {
                            prey.idleTimer = Random.Range(0f, 10f);
                        }
                    }
                }

                break;
            case PreyState.Die:
                if (prey.isDead == false)
                {
                    prey.AI.enabled = false;
                    prey.animator.SetBool("Walk", false);
                    prey.animator.SetBool("Run", false);
                    prey.animator.SetTrigger("Die");
                    prey.collider.enabled = false;
                    prey.isDead = true;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void Shoot(GameUI ui, Player player, UnityAction<HitState> completion)
    {
        var prey = player.CurrentPrey;
        player.animator.SetTrigger("Shoot");
        player.animator.SetBool("Aim", false);

        float arrowCenterDistance = ui.HuntSliderArrow.localPosition.x;
        float centerOffset = Mathf.Abs(arrowCenterDistance);
        HitState hitState;

        // Если стрелка находится в центре с учетом порога, вызываем успешную функцию
        if (centerOffset <= player.headshotCenterThreshold)
        {
            prey.collider.enabled = false;
            hitState = HitState.Headshot;
        }
        else if (centerOffset <= player.bodyCenterThreshold)
        {
            prey.collider.enabled = false;
            hitState = HitState.Body;
        }
        else
        {
            prey.collider.enabled = false;
            prey.state = PreyState.Run;
            hitState = HitState.Miss;
        }

        completion.Invoke(hitState);
        Debug.Log($"HIT {centerOffset} {hitState.ToString()}");
    }

    public static void CalculateRandomTargetPoint(ref Vector3 targetPoint, Transform target, float targetRadius)
    {
        // Вычисляем случайную точку вокруг выбранной цели
        targetPoint = target.position + new Vector3(
            Random.Range(-targetRadius, targetRadius),
            Random.Range(-targetRadius, targetRadius),
            Random.Range(-targetRadius, targetRadius)
        );
    }

    public static void LaunchArrow(Arrow arrow)
    {
        var targetPoint = arrow.targetPoint;
        var transform = arrow.transform;
        var launchAngle = arrow.launchAngle;
        var speedMultiplier = arrow.speedMultiplier;
        var rb = arrow.rb;

        // Расстояние до цели
        Vector3 direction = targetPoint - transform.position;
        float distance = direction.magnitude;

        // Преобразуем угол в радианы
        float angle = launchAngle * Mathf.Deg2Rad;

        // Вычисляем начальную скорость
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * angle)) * speedMultiplier;

        // Вычисляем компоненты скорости
        Vector3 velocityXZ = new Vector3(direction.x, 0, direction.z).normalized * velocity * Mathf.Cos(angle);
        float velocityY = velocity * Mathf.Sin(angle);

        // Устанавливаем начальную скорость стрелы
        rb.velocity = velocityXZ + Vector3.up * velocityY;

        // Поворачиваем стрелу в направлении полета
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    public static IEnumerator CutTree(Player player, Tree tree, GameManager game, GameState_SO state)
    {
        var _isNearbyPlayer = player.treesAround.Contains(tree);

        if (_isNearbyPlayer == false)
        {
            yield break;
        }

        player.progressBar.fillAmount = 0;

        for (int i = 0; i < state.currentAxe.secToCut; i++)
        {
            player.animator.SetTrigger("Chop");
            player.animator.SetBool("isChopping", true);

            yield return new WaitForSeconds(.5f);

            const float yScaling = 0.1f;
            tree.transform.DOPunchRotation(new Vector3(yScaling, yScaling, 0), 0.3f);
            player.animator.ResetTrigger("Chop");
            player.animator.SetBool("isChopping", false);
            player.ProgressCutt((i + 1) + "/5", ((float)i + 1.0f) / state.currentAxe.secToCut);

            if (i < state.currentAxe.secToCut - 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // transform.DOScaleY(0, 0.2f);
        player.StopCutt();
        tree.gameObject.SetActive(false);
        player.treesAround.Remove(tree);

        var woodCollected = state.currentAxe.logsMax;

        for (int i = 0; i < woodCollected; i++)
        {
            game.PickResources(false, player.transform, state);
            yield return new WaitForSeconds(0.1f);
        }
    }


    // Game

    public static float Countdown(ref float value)
    {
        value -= Time.deltaTime;
        if (value < 0) value = 0;

        return value;
    }

    // Scene Management

    public static void LoadSceneForLevel(int level, List<SceneReference> scenes)
    {
        int index = level - 1;
        int nLoopLevels = scenes.Count;
        index %= nLoopLevels;
        SceneManager.LoadScene(scenes[index].Name);
    }

    public static void SetResources(int meat, int wood, GameState_SO state)
    {
        state.meat += meat;
        state.wood += wood;

        // meatcountBonfire -=_meat;
        // woodcountBonFire -= _wood;

        // meatcountBonfire = Mathf.Clamp(meatcountBonfire,0, 99999);
        // woodcountBonFire = Mathf.Clamp(woodcountBonFire,0, 99999);
        // SetResourcesValue(0);
        // SetResourcesValue(1);
    }

    // UI

    public static void HuntArrowStartMoving(GameUI ui)
    {
        if (!ui.isMovingLeft)
        {
            // Начинаем движение влево
            float targetPosition = ui.HuntSliderRect.rect.width;
            ui.huntArrowMoveTween = ui.HuntSliderArrow.DOAnchorPosX(-targetPosition, ui.slideDuration)
                .SetEase(Ease.InOutQuad);
            ui.huntArrowMoveTween.OnComplete(() =>
            {
                // По завершении движения влево начинаем движение вправо
                ui.isMovingLeft = true;
                HuntArrowStartMoving(ui);
            });
        }
        else
        {
            // Начинаем движение вправо
            ui.huntArrowMoveTween = ui.HuntSliderArrow.DOAnchorPosX(0, ui.slideDuration).SetEase(Ease.InOutQuad);
            ui.huntArrowMoveTween.OnComplete(() =>
            {
                // По завершении движения вправо начинаем движение влево
                ui.isMovingLeft = false;
                HuntArrowStartMoving(ui);
            });
        }
    }

    public static void SuccessfulAction()
    {
        Debug.Log("Successful action!");
        // SmithFeedback.PlayFeedbacks();
        // SuccessSmithChannel.RaiseEvent();
    }

    public static void SpawnTeaObjects(GameObject prefab, MeshCollider terrainMeshCollider, int minTeaObjects,
        int maxTeaObjects)
    {
        Vector3 GetRandomPositionOnMesh()
        {
            // Получаем границы коллайдера
            Bounds bounds = terrainMeshCollider.bounds;

            // Пытаемся найти случайную точку на Mesh
            for (int i = 0; i < 100; i++)
            {
                // Случайная точка на поверхности Mesh
                Vector3 randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.max.y + 10, // Высота начала луча над поверхностью
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                // Raycast вниз от случайной точки
                RaycastHit hit;
                if (Physics.Raycast(randomPoint, Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.collider == terrainMeshCollider)
                    {
                        return hit.point;
                    }
                }
            }

            // Возвращаем Vector3.zero, если не удалось найти точку
            return Vector3.zero;
        }

        int teaCount = Random.Range(minTeaObjects, maxTeaObjects + 1);

        for (int i = 0; i < teaCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionOnMesh();
            if (spawnPosition != Vector3.zero)
            {
                GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public static void SpawnTrees(
        int numberOfTrees,
        GameObject groundPlane,
        LayerMask groundLayer,
        float minDistanceBetweenTrees,
        List<GameObject> treePrefabs
    )
    {
        List<Vector3> spawnedTreePositions = new List<Vector3>();

        int attempts = 0;
        int maxAttempts = 1000; // Maximum attempts to place trees to avoid infinite loop

        Bounds groundBounds = groundPlane.GetComponent<Collider>().bounds;

        while (spawnedTreePositions.Count < numberOfTrees && attempts < maxAttempts)
        {
            attempts++;

            // Generate a random position within the bounds of the ground plane
            Vector3 randomPosition = new Vector3(
                Random.Range(groundBounds.min.x, groundBounds.max.x),
                groundBounds.max.y + 10f, // Start the raycast slightly above the ground plane
                Random.Range(groundBounds.min.z, groundBounds.max.z)
            );

            // Perform a raycast downward to check if we hit the ground
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 hitPosition = hit.point;

                // Check if the position is far enough from all other spawned trees
                bool isPositionValid = true;
                foreach (Vector3 pos in spawnedTreePositions)
                {
                    if (Vector3.Distance(hitPosition, pos) < minDistanceBetweenTrees)
                    {
                        isPositionValid = false;
                        break;
                    }
                }

                // If the position is valid, spawn a tree there
                if (isPositionValid)
                {
                    GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
                    GameObject.Instantiate(treePrefab, hitPosition, Quaternion.identity);
                    spawnedTreePositions.Add(hitPosition);
                }
            }
        }

        if (spawnedTreePositions.Count < numberOfTrees)
        {
            Debug.LogWarning("Could not place all trees within the maximum number of attempts.");
        }
    }

    public static void SpawnAnimals(
        GameObject groundPlane, 
        int numberOfAnimals,
        LayerMask groundLayer,
        float minDistanceFromPlayer,
        float minDistanceBetweenAnimals,
        GameObject player,
        Prey animalPrefab,
        List<PreyType> animalTypes,
        ref List<Prey> preys
    ) {
        List<Vector3> spawnedAnimalPositions = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = 1000; // Maximum attempts to place animals to avoid infinite loop

        Bounds groundBounds = groundPlane.GetComponent<Collider>().bounds;

        while (spawnedAnimalPositions.Count < numberOfAnimals && attempts < maxAttempts)
        {
            attempts++;

            // Generate a random position within the bounds of the ground plane
            Vector3 randomPosition = new Vector3(
                Random.Range(groundBounds.min.x, groundBounds.max.x),
                groundBounds.max.y + 10f, // Start the raycast slightly above the ground plane
                Random.Range(groundBounds.min.z, groundBounds.max.z)
            );

            // Perform a raycast downward to check if we hit the ground
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 hitPosition = hit.point;

                // Check if the position is far enough from the player and other animals
                bool isPositionValid =
                    Vector3.Distance(hitPosition, player.transform.position) >= minDistanceFromPlayer;
                if (isPositionValid)
                {
                    foreach (Vector3 pos in spawnedAnimalPositions)
                    {
                        if (Vector3.Distance(hitPosition, pos) < minDistanceBetweenAnimals)
                        {
                            isPositionValid = false;
                            break;
                        }
                    }
                }

                // If the position is valid, spawn an animal there
                if (isPositionValid)
                {
                    var animalType = animalTypes[Random.Range(0, animalTypes.Count)];
                    var prey = GameObject.Instantiate(animalPrefab, hitPosition, Quaternion.identity);
                    prey.SetupPreyType(animalType);
                    spawnedAnimalPositions.Add(hitPosition);
                    preys.Add(prey);
                }
            }
        }

        if (spawnedAnimalPositions.Count < numberOfAnimals)
        {
            Debug.LogWarning("Could not place all animals within the maximum number of attempts.");
        }
    }
    
    public static bool IsPlayerInSight(Transform player, Transform transform, float fleeDistance, float fieldOfViewAngle)
    {
        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > fleeDistance) return false;

        // Проверяем угол между направлением животного и направлением на игрока
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfViewAngle / 2f) return false;

        // Проверяем, есть ли прямая видимость (с помощью Raycast)
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, fleeDistance))
        {
            if (hit.collider.transform == player)
            {
                return true; // Игрок в поле зрения
            }
        }

        return false; // Игрок не виден
    }
}