using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField]
    float backgroundBreathingTime, towerBreathingHeight, cubeTransferTime;

    int numberOfTimesFlipped = 0;

    StyleSheet styleSheet;
    Camera mainCamera;
    Color lastShapeColor;
    GameMaster gameMaster;
    [SerializeField]
    GameObject PlaneFilledParticle;
    [SerializeField]
    Color towerStartColor, towerTargetColor, backgroundStartColor, backgroundTargetColor;
    List<Color> towerColorByHeight;
    


	void Start ()
    {
        TextAsset styleSheetResources = Resources.Load("StyleSheet") as TextAsset;
        styleSheet = new StyleSheet(styleSheetResources.text);
        towerTargetColor = styleSheet.GetRandomShapeColor();
        backgroundTargetColor = styleSheet.GetRandomBackgroundColor();
        GetNewTowerTargetColor();
        GetNewBackgroundTargetColor();
        //PlaneFilledParticle = GameObject.Find("PlaneFilledEffect");

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainCamera.backgroundColor = backgroundStartColor;
        mainCamera.clearFlags = CameraClearFlags.Color;
        towerColorByHeight = new List<Color>();
        gameMaster = GetComponent<GameMaster>();
    }

    void GetNewTowerTargetColor()
    {
        towerStartColor = towerTargetColor;
        towerTargetColor = styleSheet.GetRandomShapeColor();
        while (towerTargetColor.Equals(towerStartColor))
        {
            towerTargetColor = styleSheet.GetRandomShapeColor();
        }
    }

    void GetNewBackgroundTargetColor()
    {
        backgroundStartColor = backgroundTargetColor;
        backgroundTargetColor = styleSheet.GetRandomBackgroundColor();
        while (backgroundTargetColor.Equals(backgroundStartColor))
        {
            backgroundTargetColor = styleSheet.GetRandomBackgroundColor();
        }
    }

    void ChangeShapeColor(GameObject shapeParent, Color colorToChange)
    {
        foreach (Transform cube in shapeParent.transform)
        {
            cube.gameObject.GetComponent<Renderer>().material.color = colorToChange;
        }
    }

    public void RecolorShape(GameObject shapeParent)
    {
        ChangeShapeColor(shapeParent, lastShapeColor);
    }

    public void ChangeShapeToRandomColor(GameObject shape)
    {
        lastShapeColor = styleSheet.GetRandomShapeColor();
        ChangeShapeColor(shape, lastShapeColor);
    }

    public IEnumerator breathBackground()
    {
        float currentTime = 0;
        while (currentTime < backgroundBreathingTime)
        {
            while (!gameMaster.isGameInProgress)
            {
                yield return new WaitForEndOfFrame();
            }
            mainCamera.backgroundColor = Color.Lerp(backgroundStartColor, backgroundTargetColor, currentTime / backgroundBreathingTime);
            mainCamera.clearFlags = CameraClearFlags.Color;
           // Debug.Log(mainCamera.backgroundColor);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        mainCamera.backgroundColor = backgroundTargetColor;
        GetNewBackgroundTargetColor();
        StartCoroutine(breathBackground());
    }

    public void ChangeCubeColorBasedOnHeight(GameObject cube, int height)
    {
        if (height >= towerColorByHeight.Count)
        {
            
            float fraction = height / towerBreathingHeight - numberOfTimesFlipped;

            if (fraction > 1)
            {
                fraction = 0;
                numberOfTimesFlipped++;
                GetNewTowerTargetColor();
            }
            towerColorByHeight.Add(Color.Lerp(towerStartColor, towerTargetColor, fraction));
        }
        cube.GetComponent<Renderer>().material.color = towerColorByHeight[height];
        
    }

    public IEnumerator changeIntoTowerColorGradually(GameObject cubeCollection, System.Action callbackAction)
    {
        Color startColor = cubeCollection.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
        Color targetColor = Color.Lerp(towerStartColor, towerTargetColor, cubeCollection.transform.position.y / towerBreathingHeight - numberOfTimesFlipped);
        float currentTime = 0;
        while (currentTime < cubeTransferTime)
        {
            foreach (Transform cubeTransform in cubeCollection.transform)
            {
                targetColor = Color.Lerp(towerStartColor, towerTargetColor, cubeTransform.position.y / towerBreathingHeight - numberOfTimesFlipped);
                cubeTransform.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startColor, targetColor, currentTime / cubeTransferTime);
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        callbackAction();
        
    }
    public void PlayPlaneFilledEffect(int Height)
    {
        PlaneFilledParticle.SetActive(true);
        PlaneFilledParticle.transform.position = new Vector3(0, Height - 0.5f, 0);
        PlaneFilledParticle.GetComponent<ParticleSystem>().Emit(1);
    }
}
