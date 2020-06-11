using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {


    public const string IS_AUDIO_MUTED_KEY = "Is audio muted";
    public const string IS_TUTORIAL_COMPLETED = "Is the tutorial complete";
    float timeRemainingUntilCollision;

    [SerializeField]
    int spawnHeight = 10, gameOverOffset = 3, alertOffset = 2;
    [SerializeField]
    float speedUpMultiplier;
    public int SpawnHeight { get { return spawnHeight; } }
    public int GameOverOffset { get { return gameOverOffset; } }

    GraphicsManager graphicsManager;
    public GraphicsManager GraphicsManager { get { return graphicsManager; } }
    CubeVisualizer cubeVisualizer;
    public SoundManager soundManager;
    [HideInInspector]
    public Shape currentShape;
    [HideInInspector]
    public Tower tower;
    [HideInInspector]
    public bool isGameInProgress = false;
    [HideInInspector]
    public bool isRotationInProgress = false;

    UiHandler uiHandler;
    int currentFullPlane = 0, lastHeight;
    TutorialScript tutorialScript;
    bool isTutorialInProgress = false;
    GameObject cameraOrieantator;

    void Start()
    {
        cubeVisualizer = this.GetComponent<CubeVisualizer>();
        graphicsManager = GetComponent<GraphicsManager>();
        uiHandler = GetComponent<UiHandler>();
        soundManager = GetComponent<SoundManager>();
        tower = new Tower();
        cubeVisualizer.RenderTower(tower.towerMatrix);
        isGameInProgress = false;
        Application.targetFrameRate = -1;
        cameraOrieantator = GameObject.Find("CameraOrientator");
    }


    private void Update()
    {
        if (!isGameInProgress)
        {
            return;
        }
        timeRemainingUntilCollision -= Time.deltaTime;
        if (timeRemainingUntilCollision < 0)
        {
            ShapeReachedTower();
        }
    }

    public void StartGame()
    {
        StartCoroutine(graphicsManager.breathBackground());
        ScoreManager.ResetScore();
        if (!Convert.ToBoolean(PlayerPrefs.GetInt(IS_TUTORIAL_COMPLETED)))
        {
            tutorialScript = GetComponent<TutorialScript>();
            tutorialScript.enabled = true;
            isGameInProgress = false;
            isTutorialInProgress = true;
            currentShape = ShapePool.PreChachedShapes[0];
            cubeVisualizer.RenderShape(currentShape.shapeMatrix);
            graphicsManager.ChangeShapeToRandomColor(cubeVisualizer.ShapeVisuals);
            timeRemainingUntilCollision = cubeVisualizer.calculateTimeUntilCollision();
            lastHeight = tower.Height;

        }
        else
        {
            SpawnShape();
        }    
    }

    public void ReceiveInput(Shape.Direction direction)
    {
        if (isTutorialInProgress)
        {
            tutorialScript.ReceiveInput(direction);
        }
        if (!isGameInProgress || isRotationInProgress)
        {
            return;
        }
        switch (direction)
        {
            case Shape.Direction.NONE:
                return;
            case Shape.Direction.SPEEDDOWN:
                
                break;
            case Shape.Direction.SPEEDUP:
                SpeedUp();
                break;
            default:
                RotateShape(direction);
                break;
        }
        
    }

    public void OnTutorialFinished()
    {
        PlayerPrefs.SetInt(IS_TUTORIAL_COMPLETED, Convert.ToInt32(true));
        isGameInProgress = true;
        isTutorialInProgress = false;
        SpeedUp();
    }

    public void RotateShape(Shape.Direction direction)
    {
        isRotationInProgress = true;
        currentShape.Rotate(direction);
        cubeVisualizer.ApplyShapeRotation(direction);
        cubeVisualizer.DisplayCollisionIndicator(currentShape.GetHeightMap());
        graphicsManager.RecolorShape(cubeVisualizer.ShapeVisuals);
        timeRemainingUntilCollision = cubeVisualizer.calculateTimeUntilCollision();
        soundManager.PlayRotateAudio();
    }

    void SpeedUp()
    {
        cubeVisualizer.MultiplyUnitPerSecond(speedUpMultiplier);
        timeRemainingUntilCollision = cubeVisualizer.calculateTimeUntilCollision();
    }
    void SpeedDown()
    {
        cubeVisualizer.MultiplyUnitPerSecond(1 / speedUpMultiplier);
        timeRemainingUntilCollision = cubeVisualizer.calculateTimeUntilCollision();
    }

    public void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            isGameInProgress = true;
        }
        else
        {
            isGameInProgress = false;
        }
        cubeVisualizer.PauseResumeMovement(isPaused);

    }

    public void ShapeReachedTower()
    {
       
        int numberOfPlanesFilled = tower.AddShapeToTower(currentShape);
        int newLastFullPlane = tower.LastFullPlane;
        int heightDifference = tower.Height - lastHeight ;
        lastHeight = tower.Height;
        cubeVisualizer.ResetSpeed();

        if (newLastFullPlane != currentFullPlane)
        {
            PlaneGotFilled(numberOfPlanesFilled);
            currentFullPlane = newLastFullPlane;
            graphicsManager.PlayPlaneFilledEffect(currentFullPlane);
            ScoreManager.incrementScore(heightDifference, numberOfPlanesFilled);
            soundManager.PlayPlaneFilledAudio();
        }
        else
        {
            soundManager.PlayShapeReacheAdui();
            ScoreManager.incrementScore(heightDifference, 0);
        }
        uiHandler.SetScoreDisplay(ScoreManager.Score);

        if (tower.Height > spawnHeight - gameOverOffset)
        {
            GameOver();
            return;
        }
        else if (tower.Height > spawnHeight - gameOverOffset - alertOffset)
        {
            cubeVisualizer.DisplayGameOverAlert();
        }
        else
        {
            cubeVisualizer.RemoveGameOverAlert();
        }
        cubeVisualizer.TransferShapeVisualsIntoTower();
        SpawnShape();
        
    }

    public void GameOver()
    {
        
        cubeVisualizer.GameOverEffect();
        isGameInProgress = false;
        ScoreManager.UpdateHighScore();
        uiHandler.GameOver();
        soundManager.PlayGameOverAudio();
        StartCoroutine(StartRotatingCamera(5f));
    }

    IEnumerator StartRotatingCamera(float waitTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(cubeVisualizer.AnimateRotation(
            cameraOrieantator.transform,
            270f,
            true,
            10,
            () => { StartCoroutine(StartRotatingCamera(0.01f)); } )
           );
    }

    void SpawnShape()
    {
        currentShape = ShapePool.getRandomShapeFromBag();
        cubeVisualizer.RenderShape(currentShape.shapeMatrix);
        graphicsManager.ChangeShapeToRandomColor(cubeVisualizer.ShapeVisuals);
        timeRemainingUntilCollision = cubeVisualizer.calculateTimeUntilCollision();
        lastHeight = tower.Height;
    }

    public void PlaneGotFilled(int difference)
    {
        StartCoroutine(uiHandler.PlusPointsFade(difference));
        spawnHeight += difference;
        cubeVisualizer.AnimatePlaneFilling(difference);
    }
}
