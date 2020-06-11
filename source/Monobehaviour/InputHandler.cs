using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private delegate Shape.Direction InputAction();

    InputAction inputMethodToUse;
    private GameMaster gameMaster;
	
	void Start ()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            inputMethodToUse = AndroidInput;
        }
        else
        {
            inputMethodToUse = DesktopInput;
        }
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
    }
	
	
	void Update ()
    {
        gameMaster.ReceiveInput(inputMethodToUse());
	}
    bool isSpeedUp = false;
    private Shape.Direction DesktopInput()
    {
        if (Input.GetKeyDown(KeyCode.D)) return Shape.Direction.RIGHT;

        else if (Input.GetKeyDown(KeyCode.A)) return Shape.Direction.LEFT;

        else if (Input.GetKeyDown(KeyCode.W)) return Shape.Direction.UP;

        else if (Input.GetKeyDown(KeyCode.S)) return Shape.Direction.DOWN;

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            isSpeedUp = true;
            return Shape.Direction.SPEEDUP;
        }

        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isSpeedUp = false;
            return Shape.Direction.SPEEDDOWN;
        }

        else return Shape.Direction.NONE;
        

    }

    Vector2 startPos;
    Vector2 direction;
    bool directonChosen;
    float touchTime = 0f;
    float stationaryTime = 0f;
    int stationaryTouchCount = 0;
    private Shape.Direction AndroidInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            touchTime += touch.deltaTime;
            Debug.Log(touch.phase);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    direction = Vector2.zero;
                    touchTime = 0;
                    directonChosen = false;
                    return Shape.Direction.NONE;
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    Debug.Log(direction.magnitude);
                    if (direction.magnitude < 50)
                    {
                        stationaryTime += touch.deltaTime;
                    }
                    return Shape.Direction.NONE;
                case TouchPhase.Ended:
                    if (direction.magnitude < 50)
                    {
                        direction = Vector2.zero;
                        stationaryTouchCount++;
                        if (stationaryTouchCount == 2)
                        {
                            stationaryTouchCount = 0;
                            return Shape.Direction.SPEEDUP;
                        }

                        return Shape.Direction.NONE;
                    }
                    stationaryTouchCount = 0;
                    stationaryTime = 0;
                    directonChosen = true;
                    break;
                case TouchPhase.Stationary:
                    stationaryTime += touch.deltaTime;
                    break;
            }
            
        }
        if (directonChosen)
        {
            if (touchTime < 0.05f)
            {
                return Shape.Direction.NONE;
            }
            directonChosen = false;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    return Shape.Direction.RIGHT;
                }
                else
                {
                    return Shape.Direction.LEFT;
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    return Shape.Direction.UP;
                }
                else
                {
                    return Shape.Direction.DOWN;
                }
            }
        }


        else
        {
            return Shape.Direction.NONE;
        }
        

    }

    


}
