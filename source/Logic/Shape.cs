using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape
{

    public enum Direction { UP, DOWN, LEFT, RIGHT, NONE, SPEEDUP, SPEEDDOWN }
    public List<List<List<bool>>> shapeMatrix;

    int currentDirection = 0;
    public int CurrentDirection{get { return currentDirection; }}
    bool verticalRotationPlane = true;
    public bool VerticalRotationPlane { get { return verticalRotationPlane; } }
    const int ORIGIN_OFFSET = 1;
    const float RIGHT_ANGLE_IN_RADIANS = Mathf.PI / 2;
    

    public Shape(List<List<List<bool>>> shapeMatrix)
    {
        this.shapeMatrix = shapeMatrix;
    }

    public void Rotate(Direction direction)
    {
        CalculateShapeDirection(direction);
        List<List<List<bool>>> newShapeMatrix = IntizalizeEmptyShape();
        float isReversed = 1;
        if (direction == Direction.DOWN || direction == Direction.RIGHT)
        {
            isReversed = -1;
        }
        if (direction == Direction.DOWN || direction == Direction.UP)
        {
            if (currentDirection > 1)
            {
                isReversed *= -1;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int n = 0; n < 3; n++)
                {
                    int x = i;
                    int y = j;
                    int z = n;

                    if (direction == Direction.LEFT || direction == Direction.RIGHT)
                    {
                        RotatePoint(ref y, ref z, RIGHT_ANGLE_IN_RADIANS * isReversed);
                        verticalRotationPlane ^= true;
                    }
                    else
                    {
                        if (verticalRotationPlane)
                        {
                            RotatePoint(ref x, ref y, RIGHT_ANGLE_IN_RADIANS * isReversed);
                        }
                        else
                        {
                            RotatePoint(ref x, ref z, RIGHT_ANGLE_IN_RADIANS * isReversed);
                        }
                    }
                   
                    newShapeMatrix[x][y][z] = shapeMatrix[i][j][n];
                }
               
            }
           
        }
        shapeMatrix = newShapeMatrix;
    }
    private List<List<List<bool>>> IntizalizeEmptyShape()
    {
        List<List<List<bool>>> newShapeMatrix = new List<List<List<bool>>>();
        for (int i = 0; i < 3; i++)
        {

            List<List<bool>> plane = new List<List<bool>>();
            for (int j = 0; j < 3; j++)
            {
                List<bool> line = new List<bool>();
                for (int n = 0; n < 3; n++)
                {
                    line.Add(false);
                }
                plane.Add(line);
            }
            newShapeMatrix.Add(plane);
        }

        return newShapeMatrix;


    }

    private void RotatePoint(ref int x, ref int y, float radiansToRotate)
    {
        x -= ORIGIN_OFFSET;
        y -= ORIGIN_OFFSET;

        int xOriginal = x;
        int yOriginal = y; 

        x = xOriginal * (int)Mathf.Cos(radiansToRotate) - yOriginal * (int)Mathf.Sin(radiansToRotate);
        y = yOriginal * (int)Mathf.Cos(radiansToRotate) + xOriginal * (int)Mathf.Sin(radiansToRotate);

        x += ORIGIN_OFFSET;
        y += ORIGIN_OFFSET;
    }


    public int[,] GetHeightMap()
    {
        int[,] heightMap = new int[3, 3];

        for (int j = 0; j < 3; j++)
        {
            for (int n = 0; n < 3; n++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (shapeMatrix[i][j][n])
                    {
                        heightMap[j, n] = i + 1;
                        break;
                    }
                }

            }
        }

        return heightMap;

    }

    void CalculateShapeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.LEFT:
                currentDirection++;
                break;
            case Direction.RIGHT:
                currentDirection--;
                break;
            default:
                return;
        }
        if (currentDirection == 4)
        {
            currentDirection = 0;
        }
        else if (currentDirection == -1)
        {
            currentDirection = 3;
        }
    }

	
}
