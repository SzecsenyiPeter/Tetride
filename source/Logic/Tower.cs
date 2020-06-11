using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower
{
    public List<List<List<bool>>> towerMatrix;
    int lastFullPlane = 0;

    public int Height { get { return towerMatrix.Count; } }
    public int LastFullPlane { get { return lastFullPlane; } }


    public Tower()
    {
        towerMatrix = new List<List<List<bool>>>();
        List<List<bool>> plane = new List<List<bool>>();
        for (int j = 0; j < 3; j++)
        {
           List<bool> line = new List<bool>();
           for (int n = 0; n < 3; n++)
           {
                line.Add(true);
           }
           plane.Add(line);
        }
        towerMatrix.Add(plane);
    }

    private int[,] GetHeightMap()
    {
        int[,] heightMap = new int[3, 3];

        for (int j = 0; j < 3; j++)
        {
            for (int n = 0; n < 3; n++)
            {
                for (int i = this.Height - 1; i >= 0; i--)
                {
                    if (towerMatrix[i][j][n])
                    {
                        heightMap[j, n] = i - this.Height + 1;
                        break;
                    }
                }

            }
        }

        return heightMap;
    }

    private int[,] CalculateCollsionMap(int[,] shapeHeightMap)
    {
        int[,] collisionMap = new int[3,3];
        int[,] towerHeightMap = this.GetHeightMap();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (shapeHeightMap[i,j] == 0)
                {
                    collisionMap[i, j] = int.MaxValue;
                }
                else
                {
                    collisionMap[i, j] = shapeHeightMap[i, j] - towerHeightMap[i, j];
                }
                
            }
        }
        return collisionMap;


    }

    public int getShortestDistance(int[,] shapeHeightMap)
    {
        int[,] collisionMap = CalculateCollsionMap(shapeHeightMap);
        int shortestDistance = int.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (collisionMap[i,j] < shortestDistance && collisionMap[i,j] != int.MaxValue)
                {
                    shortestDistance = collisionMap[i, j];
                }
            }
        }
        return shortestDistance;


    }

    private void AddEmptyPlain()
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
        towerMatrix.Add(plane);
    }


    public int AddShapeToTower(Shape shape)
    {
        int shortestDistance = getShortestDistance(shape.GetHeightMap());
        int planeIndexToPlaceCube = this.Height - shortestDistance + 2;
        for (int i = 0; i < 3; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (shape.shapeMatrix[i][n][j])
                    {
                        if (planeIndexToPlaceCube + i > this.Height)
                        {
                            AddEmptyPlain();
                        }
                        towerMatrix[planeIndexToPlaceCube - 1 + i][n][j] = true;
                    }
                }
            }
        }
        return FindLastFullPlane();


    }

    private int FindLastFullPlane()
    {
        int numberOfPlanesFilled = 0;
        for (int i = lastFullPlane + 1; i < Height; i++)
        {
            if (CheckIfPlaneIsFull(towerMatrix[i]))
            {
                numberOfPlanesFilled++;
                lastFullPlane = i;
            }
        }
        return numberOfPlanesFilled;
    }

    private bool CheckIfPlaneIsFull(List<List<bool>> plane)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                if (!plane[i][n])
                {
                    return false;
                }
            }
        }

        return true;
    }



}
