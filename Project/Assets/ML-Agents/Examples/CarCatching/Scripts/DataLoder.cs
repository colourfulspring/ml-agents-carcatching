using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class CapturePosMap
{
    // Read the JSON file content
    string jsonContent;

    private Dictionary<string, double[,]> resultMap;

    public CapturePosMap(string relativePath)
    {
        // Use Application.streamingAssetsPath for files in the StreamingAssets folder
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        Debug.Log("Json file at: " + fullPath);

        if (File.Exists(fullPath))
        {
            jsonContent = File.ReadAllText(fullPath);
        }
        else
        {
            Debug.Log("File not found: " + fullPath);
        }

        Debug.Log(jsonContent);

        // Deserialize the JSON content into a Dictionary<string, double[,]>
        resultMap = JsonConvert.DeserializeObject<Dictionary<string, double[,]>>(jsonContent);
    }

    // Here 'width' and 'height' mentioned below means the longer edge and the shorter edge of the scene, respectively.
    // In json file, a capturing point or an enemy point [a, b] means the height axis coordination 'a' and the width axis coordination 'b'.
    // However, in C# Astar class, a point [a, b] represents the width axis coordination 'a' and the height axis coordination 'b'.
    // Since the order of the above two formats are different, so it is necessary to change the coordination order of each point here,
    // including enemy point (constructing key) and capturing point (generating return value).
    // Moreover, Unity scene's origin is lower left corner and Python image library usually treat upper left corner as origin,
    // so suppose enemy point in costmap is [a, b], another transform to [a, 100-b] should be applied on it.
    // In conclusion, this function's input point [x, y] should be transformed to [100-y, x] first before querying
    // the json file to retrieve the corresponding capturing point. After querying, the capturing point [x, y] should also be
    // transformed to [y, 100-x]. 100 is the height of costmap.

    public double[,] getCapturePosFromEnemyPos(int enemyPosX, int enemyPosY)
    {
        (enemyPosX, enemyPosY) = (100 - enemyPosY, enemyPosX);
        string key = "(" + enemyPosX + ", " + enemyPosY + ")";
        // Debug.Log("getValueFromKey: " + key);
        double[,] posList;
        if (resultMap.ContainsKey(key))
        {
            // A new copy of the value in the dictionary should be created because the swap operation mentioned below
            // are not supposed to edit the value in the Dictionary container.
            posList = new double[3, 2];
            // .Length property is the total length of 2-dim array.
            Array.Copy(resultMap[key], posList, resultMap[key].Length);

            for (int i = 0; i < resultMap[key].GetLength(0); ++i)
            {
                (posList[i, 0], posList[i, 1]) = (posList[i, 1], 100 - posList[i, 0]);
            }
        }
        else
        {
            posList = new double[0, 0];
            Console.WriteLine($"Key '{key}' not found in the dictionary.");
        }

        // Debug.Log("getValueFromKey End");
        return posList;
    }
}

// Assume the json data has the following formats:
// Timestamp, blue car (x, z, rotation)
public class TrajList
{
    // Read the JSON file content
    string jsonContent;

    public List<Tuple<int, float[,]>> TrajDataList;

    public TrajList(string relativePath)
    {
        // Use Application.streamingAssetsPath for files in the StreamingAssets folder
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        Debug.Log("Json file at: " + fullPath);

        if (File.Exists(fullPath))
        {
            jsonContent = File.ReadAllText(fullPath);
        }
        else
        {
            Debug.Log("File not found: " + fullPath);
        }

        Debug.Log(jsonContent);

        // Deserialize the JSON content into a SortedDictionary<string, double[,]>
        SortedDictionary<string, float[,]> resultMap;
        resultMap = JsonConvert.DeserializeObject<SortedDictionary<string, float[,]>>(jsonContent);

        // Traverse the SortedDictionary and add each double[,] to the list
        TrajDataList = new List<Tuple<int, float[,]>>();
        foreach (KeyValuePair<string, float[,]> kvp in resultMap)
        {
            int timestamp = int.Parse(kvp.Key);
            TrajDataList.Add(Tuple.Create(timestamp, kvp.Value));
            // Debug.Log("timestamp " + kvp.Key);
        }

        for (int i = 0; i < TrajDataList.Count - 1; i++)
        {
            // Debug.Log("timestamp " + TrajDataList[i + 1].Item1 + " " + TrajDataList[i].Item1);
            TrajDataList[i] = Tuple.Create(TrajDataList[i + 1].Item1 - TrajDataList[i].Item1, TrajDataList[i].Item2);
            // Debug.Log("period " + TrajDataList[i].Item1);
        }
        Debug.Log("TrajList finished");
    }
}
