using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using Collections.Hybrid.Generic;

public class MyARAnchorManager
{

    public Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

    public MyARAnchorManager()
    {
        planeAnchorMap = new Dictionary<string, ARPlaneAnchorGameObject>();
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;

    }


    public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        GameObject go = UnityARUtility.CreatePlaneInScene(arPlaneAnchor);
        go.AddComponent<DontDestroyOnLoad>();  //this is so these GOs persist across scene loads
        ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject();
        arpag.planeAnchor = arPlaneAnchor;
        arpag.gameObject = go;
        planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);
    }

    public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
        {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            GameObject.Destroy(arpag.gameObject);
            planeAnchorMap.Remove(arPlaneAnchor.identifier);
        }
    }

    public void RemoveAccordingToID(string planeIdentifier) {
        if (planeAnchorMap.ContainsKey(planeIdentifier))
        {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[planeIdentifier];
            GameObject.Destroy(arpag.gameObject);
            planeAnchorMap.Remove(planeIdentifier);
            Debug.Log(planeIdentifier + " removed!");
        }
    }


    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
        {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            UnityARUtility.UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
            arpag.planeAnchor = arPlaneAnchor;
            planeAnchorMap[arPlaneAnchor.identifier] = arpag;
        }
    }

    public void UnsubscribeEvents()
    {
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent -= RemoveAnchor;
    }

    public void Destroy()
    {
        foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors())
        {
            GameObject.Destroy(arpag.gameObject);
        }

        planeAnchorMap.Clear();
        UnsubscribeEvents();
    }

    public Dictionary<string, ARPlaneAnchorGameObject>.ValueCollection GetCurrentPlaneAnchors()
    {
        return planeAnchorMap.Values;
    }

    public int GetLength()
    {
        return planeAnchorMap.Count;
    }
}


