using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    const int PLANE_FILLED_SCORE_BONUS = 5;
    const string HIGHSCORE_KEY = "highcsore";

    static int score = 0;
    public static int Score { get { return score; } }
    static int highScore;
    public static bool isThereANewHighscore = false;
    public static int HighScore { get { return PlayerPrefs.GetInt(HIGHSCORE_KEY, 0); } }

    static ScoreManager()
    {
        highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
    }

    public static void incrementScore(int heightIncrease, int numberOfPlanesFilled)
    {
        score += heightIncrease + numberOfPlanesFilled * PLANE_FILLED_SCORE_BONUS;
    }

    public static void ResetScore()
    {
        score = 0;
    }

    public static bool UpdateHighScore()
    {
        if (score > highScore)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, score);
            highScore = score;
            isThereANewHighscore = true;
            return true;
        }
        else
        {
            return false;
        }
    }


}
