using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CubeVisualizer : MonoBehaviour {

    [SerializeField]
    GameObject cubePreFab, collisionIndicatorPrefab, gameOverAlertPrefab, gameOverCheck;
    [SerializeField]
    float rotationLength, verticalOffset = 0, unitsPerSecond, cameraTransitionLength, explosionForce, quickSpeedUnitsPerSecond;
    GameMaster gameMaster;
    GameObject cameraOrieantator;
    GameObject shapeVisuals, towerVisuals, indicatorVisuals, gameOverAlertVisuals;
    public GameObject ShapeVisuals { get { return shapeVisuals; } }
    float lastPosition = 0;
    float unitSpeedToRestore;

    float targetPosition;
    float timeUntilCollision;
    int a;
	void Start ()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        cameraOrieantator = GameObject.Find("CameraOrientator");
        unitSpeedToRestore = unitsPerSecond;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!gameMaster.isGameInProgress)
        {
            return;
        }
        timeUntilCollision -= Time.deltaTime;
        moveShapeToPointOfCollision(
            gameMaster.tower.getShortestDistance(gameMaster.currentShape.GetHeightMap()),
            gameMaster.tower.Height, 
            Time.deltaTime * unitsPerSecond
        );


    }

    public void RenderShape(List<List<List<bool>>> shape, bool useCurrentPosition = false)
    {
        shapeVisuals = new GameObject("shape");
        float offSet = gameMaster.SpawnHeight;
        if (useCurrentPosition)
        {
            offSet = lastPosition;
        }      
        shapeVisuals.transform.position = new Vector3(0, offSet, 0);
        for (int i = 0; i < shape.Count; i++)
        {
            for (int n = 0; n < shape[i].Count; n++)
            {
                for (int j = 0; j < shape[i][n].Count; j++)
                {
                    if (shape[i][n][j])
                    {
                        GameObject cubeVisuals = Instantiate(cubePreFab, new Vector3(j - 1, i + offSet - 1, n - 1), Quaternion.identity);
                        cubeVisuals.transform.parent = shapeVisuals.transform;
                    }
                }
            }
        }
        StackTrace stackTrace = new StackTrace();
        print("stackTrace !! " + stackTrace.GetFrame(1).GetMethod().Name);
    }

    public void RenderTower(List<List<List<bool>>> tower)
    {
        towerVisuals = new GameObject();
        towerVisuals.transform.position = new Vector3(0, 0, 0);
        for (int i = 0; i < tower.Count; i++)
        {
            for (int n = 0; n < tower[i].Count; n++)
            {
                for (int j = 0; j < tower[i][n].Count; j++)
                {
                    if (tower[i][n][j])
                    {
                        GameObject cubeVisuals = Instantiate(cubePreFab, new Vector3(j - 1, i - 1, n - 1), Quaternion.identity);
                        gameMaster.GraphicsManager.ChangeCubeColorBasedOnHeight(cubeVisuals, i);
                        cubeVisuals.transform.parent = towerVisuals.transform;
                    }
                }
            }
        }
    }

    public void ApplyShapeRotation(Shape.Direction direction)
    {
        switch (direction)
        {
            case Shape.Direction.UP:
                StartCoroutine(AnimateRotation(shapeVisuals.transform, 90, false, rotationLength, OnRotationEnded));
                break;
            case Shape.Direction.DOWN:
                StartCoroutine(AnimateRotation(shapeVisuals.transform, -90, false, rotationLength, OnRotationEnded));
                break;
            case Shape.Direction.LEFT:
                StartCoroutine(AnimateRotation(shapeVisuals.transform, 90, true, rotationLength, OnRotationEnded));
                StartCoroutine(AnimateRotation(cameraOrieantator.transform, 90, true, rotationLength, OnRotationEnded));
                break;
            case Shape.Direction.RIGHT:
                StartCoroutine(AnimateRotation(shapeVisuals.transform, -90, true, rotationLength, OnRotationEnded));
                StartCoroutine(AnimateRotation(cameraOrieantator.transform, -90, true, rotationLength, OnRotationEnded));
                break;
        }

    }

    void OnRotationEnded()
    {
        RemoveShapeVisuals();
        RenderShape(gameMaster.currentShape.shapeMatrix, true);
        gameMaster.isRotationInProgress = false;
        gameMaster.GraphicsManager.RecolorShape(shapeVisuals);
    }

    public void RemoveShapeVisuals()
    {
        lastPosition = shapeVisuals.transform.position.y;
        Destroy(shapeVisuals);
    }

    GameObject inAnimationShape;
    public void TransferShapeVisualsIntoTower()
    {
        inAnimationShape = shapeVisuals;
        shapeVisuals.transform.position = new Vector3(shapeVisuals.transform.position.x, targetPosition, shapeVisuals.transform.position.z);
        StartCoroutine(gameMaster.GraphicsManager.changeIntoTowerColorGradually(inAnimationShape, TransferDoneCallback));
    }

    public void TransferDoneCallback()
    {
        RemoveTowerVisuals();
        RenderTower(gameMaster.tower.towerMatrix);
        Destroy(inAnimationShape);
        
    }

    public void RemoveTowerVisuals()
    {
        Destroy(towerVisuals);
    }

    public IEnumerator AnimateRotation(Transform transformToRotate, float rotation, bool isYAxis, float rotationDuration, System.Action callback)
    {
        Quaternion goal;
        if (isYAxis)
        {
            goal = Quaternion.Euler(
                transformToRotate.rotation.eulerAngles.x,
                transformToRotate.rotation.eulerAngles.y + rotation,
                transformToRotate.rotation.eulerAngles.z
                );   
        }
        else
        {
            float orientationModifier = 1;
            if (gameMaster.currentShape.CurrentDirection > 1)
            {
                orientationModifier = -1;
            }

            if (gameMaster.currentShape.VerticalRotationPlane)
            {
                goal = Quaternion.Euler(
                transformToRotate.rotation.eulerAngles.x + rotation * orientationModifier,
                transformToRotate.rotation.eulerAngles.y,
                transformToRotate.rotation.eulerAngles.z
                );
            }
            else
            {
                goal = Quaternion.Euler(
               transformToRotate.rotation.eulerAngles.x,
               transformToRotate.rotation.eulerAngles.y,
               transformToRotate.rotation.eulerAngles.z - rotation * orientationModifier
               );
            }
        }
        
        Quaternion start = transformToRotate.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transformToRotate.rotation = Quaternion.Slerp(start, goal, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transformToRotate.rotation = goal;
        callback();
       

    }
    
    public void DisplayCollisionIndicator(int[,] shapeHeightMap)
    {
        Destroy(indicatorVisuals);
        indicatorVisuals = new GameObject("IndicatorVisuals");
        for (int n = 0; n < 3; n++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (shapeHeightMap[n,j] != 0)
                {
                    for (int i = gameMaster.tower.Height - 1; i >= 0; i--)
                    {
                        if (gameMaster.tower.towerMatrix[i][n][j])
                        {
                            GameObject collsisonIndicator = Instantiate(collisionIndicatorPrefab);
                            collsisonIndicator.transform.position = new Vector3(j - 1, i - 0.5f, n - 1);
                            collsisonIndicator.transform.parent = indicatorVisuals.transform;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void DisplayGameOverAlert()
    {
        RemoveGameOverAlert();
        gameOverAlertVisuals = Instantiate(gameOverAlertPrefab);
        gameOverAlertVisuals.transform.position = new Vector3(
            0,
            gameMaster.SpawnHeight - gameMaster.GameOverOffset - 1f,
            0
            );
        Renderer renderer = gameOverAlertVisuals.GetComponent<Renderer>();
        StartCoroutine(GameOverAlertFading(
            renderer,
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0),
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a)
            ));
    }



    public void RemoveGameOverAlert()
    {
        if (gameOverAlertVisuals == null) return;
        GameObject save = gameOverAlertVisuals;
        Renderer renderer = save.GetComponent<Renderer>();
        StartCoroutine(GameOverAlertFading(
            renderer,
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a),
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0),
            delegate () { Destroy(save); }
            ));
    }

    IEnumerator GameOverAlertFading(Renderer renderer, Color start, Color target, System.Action callback = null)
    {
        float animationTime = 1f;
        float totalTime = 0f;
        renderer.material.color = start;
        while (animationTime > totalTime)
        {
            totalTime += Time.deltaTime;
            float fraction = totalTime / animationTime;
            renderer.material.color = Color.Lerp(start, target, fraction);
            yield return null;
        }
        if (callback != null)
        {
            callback();
        }
    }

    private float calculateWorldPositionTargetForShape(int shortestDistance, int towerHeight)
    {
        return towerHeight - shortestDistance + 1;
    }

    public float calculateTimeUntilCollision()
    {
        targetPosition = calculateWorldPositionTargetForShape(gameMaster.tower.getShortestDistance(gameMaster.currentShape.GetHeightMap()), gameMaster.tower.Height);
        float currentPosition = shapeVisuals.transform.position.y;
        float distance = currentPosition - targetPosition;
        DisplayCollisionIndicator(gameMaster.currentShape.GetHeightMap());
        timeUntilCollision = distance / unitsPerSecond;
        return timeUntilCollision;
    }

    private void moveShapeToPointOfCollision(int shortestDistance, int towerHeight, float distanceToMove)
    {
        if (timeUntilCollision < 0) return;
        shapeVisuals.transform.Translate(new Vector3(0, -distanceToMove, 0), Space.World);
        //shapeVisuals.transform.position = new Vector3(0, calculateWorldPositionTargetForShape(shortestDistance, towerHeight), 0);

    }

    public void AnimatePlaneFilling(int difference)
    {
        StartCoroutine(moveCameraUpSmoothly(difference));
    }

    public IEnumerator moveCameraUpSmoothly(int distance)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = cameraOrieantator.transform.position;
        Vector3 targetPosition = new Vector3(
            startPosition.x,
            startPosition.y + distance,
            startPosition.z
            );

        while (elapsedTime < cameraTransitionLength)
        {
            cameraOrieantator.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsedTime / cameraTransitionLength
                );
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cameraOrieantator.transform.position = targetPosition;
    }

    public void MultiplyUnitPerSecond(float multiplier)
    {
        unitsPerSecond = quickSpeedUnitsPerSecond;
    }

    public void PauseResumeMovement(bool isPaused)
    {
        if (isPaused)
        {
            unitsPerSecond = unitSpeedToRestore;
        }
        else
        {
            unitSpeedToRestore = unitsPerSecond;
            unitsPerSecond = 0;
        }
    }

    public void GameOverEffect()
    {
        RemoveGameOverAlert();
        int heightForKnockDown = gameMaster.tower.Height - 3 - 3;
        //Debug.Break();
        foreach (Transform towerCubeTransform in towerVisuals.transform)
        {
            Rigidbody towerCubeRigidBody = towerCubeTransform.gameObject.AddComponent<Rigidbody>();
            towerCubeRigidBody.useGravity = false;
            towerCubeRigidBody.isKinematic = true;
            if (towerCubeTransform.position.y > heightForKnockDown)
            {
                towerCubeRigidBody.useGravity = true;
                towerCubeRigidBody.isKinematic = false;
                towerCubeRigidBody.mass = 5;
                towerCubeRigidBody.AddExplosionForce(explosionForce, new Vector3(0, gameMaster.tower.Height, 0), 10);
            }
        }
        Destroy(indicatorVisuals);
        foreach (Transform shapeCubeTransform in shapeVisuals.transform)
        {
            Rigidbody shapeCubeRigidBody = shapeCubeTransform.gameObject.AddComponent<Rigidbody>();
           
            shapeCubeRigidBody.useGravity = true;
            shapeCubeRigidBody.isKinematic = false;
            shapeCubeRigidBody.mass = 5;
            shapeCubeRigidBody.AddExplosionForce(explosionForce, new Vector3(4, heightForKnockDown + 2, 0), 10);
        }

        StartCoroutine(CameraZoomOut());

    }

    IEnumerator CameraZoomOut()
    {
        Vector3 visibilityCords = Camera.main.WorldToScreenPoint(gameOverCheck.transform.position);
        while (!((visibilityCords.x >= 0 && visibilityCords.y >= 0) && (visibilityCords.x <= Camera.main.pixelWidth && visibilityCords.y <= Camera.main.pixelHeight) && visibilityCords.z >= 0))
        {
            visibilityCords = Camera.main.WorldToScreenPoint(gameOverCheck.transform.position);
            Camera.main.transform.Translate(new Vector3(0, 0, -0.1f));
            yield return new WaitForSeconds(0.01f);
        }


    }

    public void ResetSpeed()
    {
        unitsPerSecond = unitSpeedToRestore;
    }
}
