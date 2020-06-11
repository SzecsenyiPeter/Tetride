using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialScript : MonoBehaviour {

    struct TutorialStageInfo
    {
        public string animationName;
        public Shape.Direction directionToProgress;
    }

    [SerializeField]
    GameObject tutorialUi;

    Animator touchAnimatior;
    int currentStage = 0;
    TutorialStageInfo[] tutorialStages;
    GameMaster gameMaster;

   
	void Start ()
    {
        tutorialUi.SetActive(true);
        touchAnimatior = GameObject.Find("Touch").GetComponent<Animator>();
        gameMaster = GetComponent<GameMaster>();

        tutorialStages = new TutorialStageInfo[5];
        tutorialStages[0] = new TutorialStageInfo();
        tutorialStages[0].directionToProgress = Shape.Direction.LEFT;
        tutorialStages[0].animationName = "swipe_left";

        tutorialStages[1] = new TutorialStageInfo();
        tutorialStages[1].directionToProgress = Shape.Direction.RIGHT;
        tutorialStages[1].animationName = "swipe_right";

        tutorialStages[2] = new TutorialStageInfo();
        tutorialStages[2].directionToProgress = Shape.Direction.UP;
        tutorialStages[2].animationName = "swipe_up";

        tutorialStages[3] = new TutorialStageInfo();
        tutorialStages[3].directionToProgress = Shape.Direction.DOWN;
        tutorialStages[3].animationName = "swipe_down";

        tutorialStages[4] = new TutorialStageInfo();
        tutorialStages[4].directionToProgress = Shape.Direction.SPEEDUP;
        tutorialStages[4].animationName = "touch_hold";

        touchAnimatior.Play(tutorialStages[currentStage].animationName);
        
    }
	
	

    public void ReceiveInput(Shape.Direction direction)
    {
        if (direction == tutorialStages[currentStage].directionToProgress)
        {
            currentStage++;
            if (currentStage == 5)
            {
                gameMaster.OnTutorialFinished();
                tutorialUi.SetActive(false);
            }
            else
            {
                gameMaster.RotateShape(direction);
                touchAnimatior.Play(tutorialStages[currentStage].animationName);
            }
            
        }
    }

    
}
