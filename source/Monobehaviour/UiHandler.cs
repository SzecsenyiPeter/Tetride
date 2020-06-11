using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class UiHandler : MonoBehaviour
{
    [SerializeField]
    GameObject startUi, ingameUI, gameOverUi, pauseUi, mainMenuAudioButton, pauseMenuAudioButton, creditUi;
    [SerializeField]
    TextMeshProUGUI scoreDisplay, gameOverScoreDisplyay, highScoreDisplay;
    [SerializeField]
    Vector3 animationStartPosition, animationTargetPostion;
    [SerializeField]
    Sprite mutedImage, unMutedImage;
    Vector3 referenceVelocity = Vector3.zero;
    [SerializeField]
    float animationTime;
    GameMaster gameMaster;
    

    private void Start()
    {
        StartCoroutine(UiFadeInAnimation(startUi.GetComponent<RectTransform>(), animationStartPosition, animationTargetPostion));
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        
        UpdateAudioButtonGraphics();
    }

    public void TapToStartClicked()
    {
        
        gameMaster.isGameInProgress = true;
        //startUi.transform.Find("Button").gameObject.SetActive(false);
        startUi.GetComponent<Animator>().Play("startScreenFade");
        //StartCoroutine(UiFadeInAnimation(startUi.GetComponent<RectTransform>(), animationTargetPostion, animationStartPosition, delegate () { startUi.SetActive(false); }));
        ingameUI.SetActive(true);
        gameMaster.StartGame();
        SetScoreDisplay(0);
    }

    public void SetScoreDisplay(int score)
    {
        scoreDisplay.text = score.ToString();
    }

    public void GameOver()
    {
        gameOverUi.SetActive(true);
        StartCoroutine(UiFadeInAnimation(gameOverUi.GetComponent<RectTransform>(), animationStartPosition, animationTargetPostion, delegate() { return; }));
        ingameUI.SetActive(false);
        gameOverScoreDisplyay.text = scoreDisplay.text;
        highScoreDisplay.text = "Highscore " + ScoreManager.HighScore;
        if (ScoreManager.isThereANewHighscore)
        {
            highScoreDisplay.text = "New Best";
        }
        
    }

    public void RestartClicked()
    {
        if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
        {
            Debug.Log("Játszódna ad");
            if (Application.platform == RuntimePlatform.Android)
            {
                GetComponent<ShowAd>().DisplayAD();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        
    }

    //IEnumerator UiFadeInAnimation(RectTransform transformToMove, Vector3 startPos, Vector3 targetPos, System.Action callback = null)
    //{
    //    transformToMove.localPosition = new Vector3(startPos.x * RandomOneOrMinusOne(), startPos.y);
    //    referenceVelocity = Vector3.zero;
    //    float totalTimePassed = 0;
    //    while (transformToMove.localPosition != targetPos)
    //    {
    //        totalTimePassed += Time.deltaTime;
    //        //if (totalTimePassed > animationTime + 0.2f)
    //        //{
    //        //    transformToMove.localPosition = targetPos;
    //        //    break;
    //        //}
    //        transformToMove.localPosition = Vector3.SmoothDamp(transformToMove.localPosition, targetPos, ref referenceVelocity, animationTime);
    //        yield return new WaitForFixedUpdate();
    //    }
    //    if (callback != null)
    //    {
    //        callback();
    //    }
    //}

    IEnumerator UiFadeInAnimation(RectTransform transformToMove, Vector3 startPos, Vector3 targetPos, System.Action callback = null)
    {
        yield return new WaitForSeconds(0.1f);
        transformToMove.localPosition = new Vector3(startPos.x * RandomOneOrMinusOne(), startPos.y);
        float totalTimePassed = 0;

        while (totalTimePassed < animationTime)
        {
            totalTimePassed += Time.deltaTime;
            float fraction = totalTimePassed / animationTime;
            transformToMove.localPosition = new Vector3(Mathf.SmoothStep(transformToMove.localPosition.x, targetPos.x, fraction), 0, 0);
            yield return null;
        }
        transformToMove.localPosition = targetPos;
        if (callback != null)
        {
            callback();
        }
    }
    int RandomOneOrMinusOne()
    {
        return -1 + 2 * UnityEngine.Random.Range(0, 2);
    }
    public void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            pauseUi.SetActive(false);
        }
        else
        {
            pauseUi.SetActive(true);
        }
        gameMaster.PauseGame(isPaused);
    }

    public void onMuteClicked()
    {
        bool isAudioOn = Convert.ToBoolean(PlayerPrefs.GetInt(GameMaster.IS_AUDIO_MUTED_KEY));
        isAudioOn ^= true;
        PlayerPrefs.SetInt(GameMaster.IS_AUDIO_MUTED_KEY, Convert.ToInt32(isAudioOn));
        gameMaster.soundManager.TurnAudioOnOff();
        UpdateAudioButtonGraphics();
    }

    public void OnCreditsButtonClicked()
    {
        creditUi.SetActive(true);
        startUi.SetActive(false);
    }

    public void OnCreditsDismissed()
    {
        creditUi.SetActive(false);
        startUi.SetActive(true);
    }

    void UpdateAudioButtonGraphics()
    {
        bool isAudioOn = Convert.ToBoolean( PlayerPrefs.GetInt(GameMaster.IS_AUDIO_MUTED_KEY));
        if (isAudioOn)
        {
            mainMenuAudioButton.GetComponent<Image>().sprite = unMutedImage;
            pauseMenuAudioButton.GetComponent<Image>().sprite = unMutedImage;
        }
        else
        {
            mainMenuAudioButton.GetComponent<Image>().sprite = mutedImage;
            pauseMenuAudioButton.GetComponent<Image>().sprite = mutedImage;
        }
    }

    public IEnumerator PlusPointsFade(int difference)
    {
        TextMeshProUGUI plusPointDisplay = ingameUI.transform.Find("Plus").gameObject.GetComponent<TextMeshProUGUI>();
        plusPointDisplay.SetText("+" + (5 * difference).ToString());
        float animationTime = 0.3f;
        float totalTime = 0f;
        Color start = new Color(plusPointDisplay.color.r, plusPointDisplay.color.g, plusPointDisplay.color.b, 0);
        Color target = new Color(plusPointDisplay.color.r, plusPointDisplay.color.g, plusPointDisplay.color.b, 1);
        plusPointDisplay.color = start;
        while (animationTime > totalTime)
        {
            totalTime += Time.deltaTime;
            float fraction = totalTime / animationTime;
            plusPointDisplay.color = Color.Lerp(start, target, fraction);
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        totalTime = 0;
        while (animationTime > totalTime)
        {
            totalTime += Time.deltaTime;
            float fraction = totalTime / animationTime;
            plusPointDisplay.color = Color.Lerp(target, start, fraction);
            yield return null;
        }

    }
}
