using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleSheet
{
    public List<Color> shapeColorPool;
    public List<Color> backgroundColorPool;

    public List<Color> ShapeColorPool { get { return shapeColorPool; } }
    public List<Color> BackgroundColorPool { get { return backgroundColorPool; } }

    public StyleSheet(string styleSheetResourceString)
    {
        shapeColorPool = new List<Color>();
        backgroundColorPool = new List<Color>();
        string[] styleSheetSplit = styleSheetResourceString.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );

        int index = 1;
        while (styleSheetSplit[index] != "shape")
        {
            Color colorToAdd;
            if (ColorUtility.TryParseHtmlString(styleSheetSplit[index], out colorToAdd))
            {
                backgroundColorPool.Add(colorToAdd);
            }
            index++;
        }
        index += 1;
        while (styleSheetSplit[index] != "end")
        {
            Color colorToAdd;
            if (ColorUtility.TryParseHtmlString(styleSheetSplit[index], out colorToAdd))
            {
                shapeColorPool.Add(colorToAdd);
            }
            index++;
        }


    }

    public Color GetRandomShapeColor()
    {
        return shapeColorPool[UnityEngine.Random.Range(0, shapeColorPool.Count)];
    }

    public Color GetRandomBackgroundColor()
    {
        return backgroundColorPool[UnityEngine.Random.Range(0, backgroundColorPool.Count)];
    }

}
