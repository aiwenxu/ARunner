using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class InitializePlanes : MonoBehaviour
{

    ARSessionOrigin aRSessionOrigin;
    ARPlaneManager aRPlaneManager;

    [SerializeField]
    TMPro.TMP_Text stateText;
    [SerializeField]
    TMPro.TMP_Text planeText;

    [SerializeField]
    GameObject initializationCanvas;
    Button finishButton;

    int numOfPlanes;

    private void Awake()
    {
        aRSessionOrigin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        aRPlaneManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();

        finishButton = GameObject.Find("FinishButton").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        ARSubsystemManager.systemStateChanged += UpdateStateText;

        aRPlaneManager.planeAdded += IncrementPlaneCount;
        aRPlaneManager.planeRemoved += DecrementPlaneCount;

        finishButton.onClick.AddListener(StopPlaneDetection);
    }

    private void OnDisable()
    {
        ARSubsystemManager.systemStateChanged -= UpdateStateText;

        aRPlaneManager.planeAdded -= IncrementPlaneCount;
        aRPlaneManager.planeRemoved -= DecrementPlaneCount;
    }

    private void UpdateStateText(ARSystemStateChangedEventArgs args)
    {
        stateText.text = args.ToString();
    }

    private void IncrementPlaneCount(ARPlaneAddedEventArgs args)
    {
        numOfPlanes++;
        UpdatePlaneText();
    }

    private void DecrementPlaneCount(ARPlaneRemovedEventArgs args)
    {
        numOfPlanes--;
        UpdatePlaneText();
    }

    private void UpdatePlaneText()
    {
        string newText = "Planes: " + numOfPlanes;
        planeText.text = newText;
    }

    private void StopPlaneDetection()
    {
        aRPlaneManager.enabled = false;
        initializationCanvas.SetActive(false);
    }

}
