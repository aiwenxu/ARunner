using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  

public class GamePlayManager : MonoBehaviour
{
    // This becomes true once "start" button is pressed
    private bool gameStarted = false;

    public GameObject gamePlane;
    public GameObject gamePlaneOrigin;
    public GameObject gameCharacter;
    public GameObject[] obstacles;
    public GameObject tempRoadBlock1;
    public GameObject tempRoadBlock2;
    public GameObject tempRoadBlock3;

    public Text debugText;

    public Text pointsText;

    private bool roadBlockPlaced = false;

    // This becomes true once "run" button is pressed
    private bool startRunning = false;

    // This becomes true once the coroutine of generating the road starts.
    private bool automaticRoadStarted = false;

    public bool dead = false;

    private int basePoints = 20;
    private int timeNotOnRoad = 0;
    private int numObstaclesRunInto = 0;
    private int totalPoints = 0; 

    private float roadBlockWidth;
    private Vector3 roadBlockSize;

    // When this is true, game plane size init and obstacles in game are found.
    private bool gamePlaneSizeIsInit;

    private Vector3 gamePlaneExtent;

    private float roadBlockCenterRangeX;
    private float roadBlockCenterRangeZ;

    private System.Random rnd;

    private int stepCount = 0; // also used in computing total points

    public GameObject debugVisBall;

    SimpleCharacterControl controlScript;

    // Start is called before the first frame update
    void Start()
    {
        roadBlockWidth = tempRoadBlock1.GetComponent<Collider>().bounds.size.z;
        roadBlockSize = tempRoadBlock1.GetComponent<Collider>().bounds.size;

        rnd = new System.Random(39538479);

    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {

            if (!gamePlaneSizeIsInit)
            {
                //Mesh planeMesh = gamePlane.GetComponent<MeshFilter>().mesh;
                //planeMesh.RecalculateBounds();
                //gamePlaneExtent = planeMesh.bounds.extents;
                gamePlaneExtent = gamePlane.GetComponent<Collider>().bounds.extents;
                roadBlockCenterRangeX = gamePlaneExtent.x - roadBlockWidth / 2;
                roadBlockCenterRangeZ = gamePlaneExtent.z - roadBlockWidth / 2;

                // I put finding obstacles here too.
                // In fact, it might not be useful. Could be removed. But I'll keep them here for now...
                obstacles = GameObject.FindGameObjectsWithTag("obstacle");
                Debug.Log(obstacles.Length);

                gamePlaneSizeIsInit = true;

                controlScript = gameCharacter.GetComponent<SimpleCharacterControl>();


            }

            if (!roadBlockPlaced)
            {
                tempRoadBlock2.transform.parent = gamePlaneOrigin.transform;
                tempRoadBlock2.transform.localPosition = gameCharacter.transform.localPosition;
                //tempRoadBlock.transform.localPosition = new Vector3(gameCharacter.transform.localPosition.x, gameCharacter.transform.localPosition.y, gameCharacter.transform.localPosition.z);
                tempRoadBlock2.transform.localRotation = gameCharacter.transform.localRotation;

                tempRoadBlock1.transform.position = tempRoadBlock2.transform.position - tempRoadBlock2.transform.forward * roadBlockWidth;
                tempRoadBlock1.transform.rotation = tempRoadBlock2.transform.rotation;
                tempRoadBlock1.transform.parent = gamePlaneOrigin.transform;

                tempRoadBlock3.transform.position = tempRoadBlock2.transform.position + tempRoadBlock2.transform.forward * roadBlockWidth;
                tempRoadBlock3.transform.rotation = tempRoadBlock2.transform.rotation;
                tempRoadBlock3.transform.parent = gamePlaneOrigin.transform;

                roadBlockPlaced = true;
            }
            if (startRunning && !automaticRoadStarted)
            {
                StartCoroutine(AutomaticRoadGenerator());
                Debug.Log("coroutine started");
                automaticRoadStarted = true;
            }

            if (startRunning && !dead)
            {
                totalPoints = basePoints + stepCount * 2 - timeNotOnRoad * 3 - 10 * numObstaclesRunInto;
                pointsText.text = totalPoints.ToString();
                // TODO: disable stuff when dead
                if (totalPoints <=0)
                {
                    dead = true;
                }

                if (controlScript.enterTrigger == true)
                {
                    numObstaclesRunInto++;
                    Debug.Log(numObstaclesRunInto);
                    controlScript.enterTrigger = false;
                }
            }

            if (dead)
            {
                SceneManager.LoadScene(1);
            }


        }
    }

    // Assume the road block is a child of gamePlaneOrigin.
    // could be removed, but I don't care...
    private bool IsRoadBlockInPlane(GameObject roadBlock)
    {
        if (Math.Abs(roadBlock.transform.localPosition.x) > roadBlockCenterRangeX) {
            return false;
        }
        if (Math.Abs(roadBlock.transform.localPosition.z) > roadBlockCenterRangeZ)
        {
            return false;
        }
        return true;
    }

    private bool IsPosInPlane(Vector3 pos)
    {
        if (Math.Abs(pos.x) > roadBlockCenterRangeX)
        {
            return false;
        }
        if (Math.Abs(pos.z) > roadBlockCenterRangeZ)
        {
            return false;
        }
        return true;
    }

    private PositionAndIndex ChooseNextPos(Transform currentRoadBlock)
    {
        bool[] positionsOK = { true, true, true };

        Vector3 forwardPos = CalculateNextPosition(currentRoadBlock.position, currentRoadBlock.forward);
        Vector3 rightPos = CalculateNextPosition(currentRoadBlock.position, currentRoadBlock.right);
        Vector3 leftPos = CalculateNextPosition(currentRoadBlock.position, -currentRoadBlock.right);

        Transform gamePlaneOriginTransform = gamePlaneOrigin.transform;

        Vector3[] localPositions = { gamePlaneOriginTransform.InverseTransformPoint(forwardPos), gamePlaneOriginTransform.InverseTransformPoint(rightPos), gamePlaneOriginTransform.InverseTransformPoint(leftPos) };

        for (int i = 0; i < 3; i++)
        {
            positionsOK[i] = IsPosInPlane(localPositions[i]);
        }


        int choice = RandomChoose(positionsOK);

        PositionAndIndex posAndIdx = new PositionAndIndex(choice, localPositions[choice]);

        return posAndIdx;
    }

    private int RandomChoose(bool[] positionsOK)
    {
        int index = rnd.Next(0, 3);
        while (!positionsOK[index])
        {
            index = rnd.Next(0, 3);
        }
        return index;
    }

    private Vector3 CalculateNextPosition(Vector3 currentPosition, Vector3 direction)
    {
        return currentPosition + direction * roadBlockWidth;
    }

    public void PlaceARoadBlock()
    {
        if (stepCount % 3 == 0)
        {
            PositionAndIndex newPosAndIdx = ChooseNextPos(tempRoadBlock3.transform);
            tempRoadBlock1.transform.localPosition = newPosAndIdx.pos;
            RotateAccordingToIndexAndOldBlock(tempRoadBlock1.transform, tempRoadBlock3.transform, newPosAndIdx.idx);
        }
        else if (stepCount % 3 == 1)
        {
            PositionAndIndex newPosAndIdx = ChooseNextPos(tempRoadBlock1.transform);
            tempRoadBlock2.transform.localPosition = newPosAndIdx.pos;
            RotateAccordingToIndexAndOldBlock(tempRoadBlock2.transform, tempRoadBlock1.transform, newPosAndIdx.idx);
        }
        else
        {
            PositionAndIndex newPosAndIdx = ChooseNextPos(tempRoadBlock2.transform);
            tempRoadBlock3.transform.localPosition = newPosAndIdx.pos;
            RotateAccordingToIndexAndOldBlock(tempRoadBlock3.transform, tempRoadBlock2.transform, newPosAndIdx.idx);
        }
        stepCount++;
    }

    // Generate road and check whether character on road. Points are deducted here.
    private IEnumerator AutomaticRoadGenerator()
    {
        while (!dead)
        {
            PlaceARoadBlock();
            //if (!IsCharacterOnRoad())
            //{
            //    debugText.text = "Not on road";
            //}
            //else
            //{
            //    debugText.text = "On road";
            //}
            if (!IsCharacterOnRoad())
            {
                timeNotOnRoad++;
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void RotateAccordingToIndexAndOldBlock(Transform newBlock, Transform oldBlock, int i)
    {
        newBlock.localRotation = oldBlock.localRotation;
        if (i == 1)
        {
            newBlock.Rotate(0, 90, 0, Space.Self);
        }
        if (i == 2)
        {
            newBlock.Rotate(0, -90, 0, Space.Self);
        }
    }

    public bool IsCharacterOnRoad()
    {
        Vector3 characterPos = gameCharacter.transform.localPosition;
        //Debug.Log(roadBlockWidth);
        //Debug.Log(characterPos.ToString("F5"));
        Vector3[] roadBlockPos = { tempRoadBlock1.transform.localPosition, tempRoadBlock2.transform.localPosition, tempRoadBlock3.transform.localPosition };
        for (int i = 0; i < 3; i++)
        {
            //Debug.Log(roadBlockPos[i].ToString("F5"));
            if (Math.Abs(characterPos.x - roadBlockPos[i].x) < roadBlockWidth / 2 && Math.Abs(characterPos.z - roadBlockPos[i].z) < roadBlockWidth / 2)
            {
                //Debug.Log("on road " + i.ToString());
                return true;
            }
        }
        return false;
    }


    public void StartGame()
    {
        gameStarted = true;
    }

    public void StartRunning()
    {
        startRunning = true;
    }

    public void DebugPosition()
    {
        Debug.Log(tempRoadBlock1.transform.position.ToString("F5"));
        //Debug.Log(roadBlockWidth);
        //Debug.Log(roadBlockSize.ToString());
        Debug.Log(tempRoadBlock2.transform.position.ToString("F5"));
        Debug.Log(gamePlaneExtent);

        GameObject cornerBall1 = Instantiate(debugVisBall);
        cornerBall1.transform.parent = gamePlaneOrigin.transform;
        cornerBall1.transform.localPosition = gamePlaneExtent;

        GameObject cornerBall2 = Instantiate(debugVisBall, gamePlaneOrigin.transform);
        cornerBall2.transform.localPosition = new Vector3(gamePlaneExtent.x, gamePlaneExtent.y, -gamePlaneExtent.z);

        GameObject cornerBall3 = Instantiate(debugVisBall, gamePlaneOrigin.transform);
        cornerBall3.transform.localPosition = new Vector3(-gamePlaneExtent.x, gamePlaneExtent.y, -gamePlaneExtent.z);

        GameObject cornerBall4 = Instantiate(debugVisBall, gamePlaneOrigin.transform);
        cornerBall4.transform.localPosition = new Vector3(-gamePlaneExtent.x, gamePlaneExtent.y, gamePlaneExtent.z);

        Debug.Log(cornerBall1.transform.localPosition.ToString("F5"));
        Debug.Log(IsRoadBlockInPlane(tempRoadBlock1));
        Debug.Log(IsRoadBlockInPlane(tempRoadBlock2));

    }
}

public struct PositionAndIndex
{
    public int idx;
    public Vector3 pos;

    public PositionAndIndex(int index, Vector3 position)
    {
        idx = index;
        pos = position;
    }
}