using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ScenePositionData
{
    public string sceneName;
    public Vector3 playerPosition;
}

[CreateAssetMenu(fileName = "Position Save", menuName = "Position Save")]
public class PositionSaving : ScriptableObject
{
    public List<ScenePositionData> saveEntries = new List<ScenePositionData>();

    // This function will either update an existing scene or add a new one
    public void SavePosition(string scene, Vector3 pos)
    {
        // 1. Check if we already have an entry for this scene
        for (int i = 0; i < saveEntries.Count; i++)
        {
            if (saveEntries[i].sceneName == scene)
            {
                ScenePositionData updatedData = saveEntries[i];
                updatedData.playerPosition = pos;
                saveEntries[i] = updatedData;
                Debug.Log($"Updated position for {scene}");
                return;
            }
        }

        ScenePositionData newData = new ScenePositionData();
        newData.sceneName = scene;
        newData.playerPosition = pos;

        saveEntries.Add(newData);
        Debug.Log($"Created new save entry for {scene}");
    }
}