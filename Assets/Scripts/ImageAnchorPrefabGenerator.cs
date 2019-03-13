using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ImageAnchorPrefabGenerator : MonoBehaviour {


	[SerializeField]
	private ARReferenceImage referenceImage;

	[SerializeField]
	private GameObject prefabToGenerate;

    public GameObject obstaclePrefab;

	private GameObject imageAnchorGO;

    private ARImageAnchor currentAnchor = null;

	// Use this for initialization
	void Start () {
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

	}

	void AddImageAnchor(ARImageAnchor arImageAnchor)
	{
		//Debug.LogFormat("image anchor added[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
        currentAnchor = arImageAnchor;
		if (arImageAnchor.referenceImageName == referenceImage.imageName) {
			Vector3 position = UnityARMatrixOps.GetPosition (arImageAnchor.transform);
			Quaternion rotation = UnityARMatrixOps.GetRotation (arImageAnchor.transform);

			imageAnchorGO = Instantiate<GameObject> (prefabToGenerate, position, rotation);
		}
	}

	void UpdateImageAnchor(ARImageAnchor arImageAnchor)
	{
        //Debug.LogFormat("image anchor updated[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
        currentAnchor = arImageAnchor;
		if (arImageAnchor.referenceImageName == referenceImage.imageName) {
            if (arImageAnchor.isTracked)
            {
                if (!imageAnchorGO.activeSelf)
                {
                    imageAnchorGO.SetActive(true);
                }
                imageAnchorGO.transform.position = UnityARMatrixOps.GetPosition(arImageAnchor.transform);
                imageAnchorGO.transform.rotation = UnityARMatrixOps.GetRotation(arImageAnchor.transform);
            }
            else if (imageAnchorGO.activeSelf)
            {
                imageAnchorGO.SetActive(false);
            }
        }

	}

	void RemoveImageAnchor(ARImageAnchor arImageAnchor)
	{
		//Debug.LogFormat("image anchor removed[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
		if (imageAnchorGO) {
			GameObject.Destroy (imageAnchorGO);
		}

	}

    public void PlaceObstacle()
    {
        if (currentAnchor != null)
        {
            if (currentAnchor.isTracked)
            {
                Vector3 position = UnityARMatrixOps.GetPosition(currentAnchor.transform);
                Quaternion rotation = UnityARMatrixOps.GetRotation(currentAnchor.transform);
                GameObject newObstacle = Instantiate<GameObject> (obstaclePrefab, position, rotation);
                newObstacle.tag = "obstacle";
                Debug.Log(position.ToString("F5"));
            }
            else
            {
                Debug.Log("current anchor not tracked");
            }
        }
        else
        {
            Debug.Log("current anchor null");
        }
        Debug.Log("done");
    }

    void OnDestroy()
	{
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;
        
	}

	// Update is called once per frame
	void Update () {
		
	}
}
