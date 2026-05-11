using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Change this from 'struct' to 'class' to satisfy the native class requirement
public class ObjectPersistenceData
{
    public string objectID;
    public bool interactedWith;

    // A simple constructor makes adding new entries cleaner
    public ObjectPersistenceData(string id, bool state)
    {
        objectID = id;
        interactedWith = state;
    }
}

[CreateAssetMenu(fileName = "Object Save", menuName = "Object Save")]
public class ObjectSaving : ScriptableObject
{
    public List<ObjectPersistenceData> objectEntries = new List<ObjectPersistenceData>();

    public void SaveInteraction(string id, bool state)
    {
        for (int i = 0; i < objectEntries.Count; i++)
        {
            if (objectEntries[i].objectID == id)
            {
                // Classes are reference types, so we can edit the value directly
                objectEntries[i].interactedWith = state;
                return;
            }
        }

        // Using the new constructor
        objectEntries.Add(new ObjectPersistenceData(id, state));
    }

    public bool CheckInteraction(string id)
    {
        foreach (var entry in objectEntries)
        {
            if (entry.objectID == id) return entry.interactedWith;
        }
        return false;
    }

    public void ClearMemories() => objectEntries.Clear();
}