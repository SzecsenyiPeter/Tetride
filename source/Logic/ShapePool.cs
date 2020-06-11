using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapePool
{

    static List<Shape> preCachedShapes;

    public static List<Shape> PreChachedShapes{ get { return preCachedShapes; } } 
    public static int NumberOfPreCachedShapes { get { return preCachedShapes.Count; } }
   
    static ShapePool()
     {
       

        string[] test = new string[12];

        test[0] = ("0 0 0");
        test[1] = ("0 0 0");
        test[2] = ("0 1 0");

        test[3] = ("+");

        test[4] = ("0 0 0");
        test[5] = ("0 0 0");
        test[6] = ("0 1 0");

        test[7] = ("+");

        test[8] = ("0 0 0");
        test[9] = ("0 1 0");
        test[10] = ("0 1 0");

        test[11] = ("+");

        TextAsset shapeTextResources = Resources.Load("Shape_List") as TextAsset;
        string shapeText = shapeTextResources.text;
        string[] shapeTextSplit = shapeText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        preCachedShapes = new List<Shape>();
        string[] shapeStringArray = new string[12];
        int counter = 0;
        foreach (string line in shapeTextSplit)
        {
            if (line == "---")
            {
                preCachedShapes.Add(convertInputStringToShape(shapeStringArray));
                counter = 0;
                shapeStringArray = new string[12];
            }
            else
            {
                shapeStringArray[counter] = line;
                counter++;
            }
        }
        
        
    }
    static List<int> shapesTakenOutOfTheBag = new List<int>();
    static int lastShapeTakenOut = -1;
    public static Shape getRandomShapeFromBag() 
    {
        int selected = 0;
        if (shapesTakenOutOfTheBag.Count == NumberOfPreCachedShapes)
        {
            shapesTakenOutOfTheBag.Clear();
            do
            {
                selected = UnityEngine.Random.Range(0, NumberOfPreCachedShapes);
            } while (lastShapeTakenOut == selected);
        }
        else
        {
            do
            {
                selected = UnityEngine.Random.Range(0, NumberOfPreCachedShapes);
            } while (shapesTakenOutOfTheBag.Contains(selected));
        }
        shapesTakenOutOfTheBag.Add(selected);
        lastShapeTakenOut = selected;
        return preCachedShapes[selected];
    }
    private static Shape convertInputStringToShape(string[] shapeData)
    {
        List<List<List<bool>>> shapeMatrix = new List<List<List<bool>>>();
        int n = 0;
        for (int i = 0; i < 3; i++)
        {

            List<List<bool>> plane = new List<List<bool>>(); 
            while (n < shapeData.Length && shapeData[n] != "+")
            {
                plane.Add(convertStringToLine(shapeData[n]));
                n++;

            }
            n++;
            shapeMatrix.Add(plane);

        }

        return new Shape(shapeMatrix);
    }

    static List<bool> convertStringToLine(string lineToConvert)
    {

        string[] lineSplit = lineToConvert.Split(' ');
        List<bool> line = new List<bool>();
        for (int j = 0; j < lineSplit.Length; j++)
        {
            if (lineSplit[j] == "1")
            {
                line.Add(true);
            }
            else
            {
                line.Add(false);
            }
        }
        return line;

    }

}
