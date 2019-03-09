using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayManager : MonoBehaviour
{
    private bool gameStarted = false;
    public GameObject gamePlane;
    public GameObject gamePlaneOrigin;
    public GameObject gameCharacter;
    public GameObject[] obstacles;
    public GameObject[] lifes;
    public GameObject[] roadBlocks;
    public GameObject tempRoadBlock1;
    public GameObject tempRoadBlock2;

    public bool roadBlockPlaced = false;

    private float roadBlockWidth;
    private Vector3 roadBlockSize;

    private bool gamePlaneSizeIsInit;
    private Vector3 gamePlaneExtent;

    private float roadBlockCenterRangeX;
    private float roadBlockCenterRangeZ;

    private System.Random rnd;

    private int stepCount = 0;

    public GameObject debugVisBall;

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
                gamePlaneSizeIsInit = true;
            }

            if (!roadBlockPlaced)
            {
                tempRoadBlock1.transform.parent = gamePlaneOrigin.transform;
                tempRoadBlock1.transform.localPosition = gameCharacter.transform.localPosition;
                //tempRoadBlock.transform.localPosition = new Vector3(gameCharacter.transform.localPosition.x, gameCharacter.transform.localPosition.y, gameCharacter.transform.localPosition.z);
                tempRoadBlock1.transform.localRotation = gameCharacter.transform.localRotation;

                tempRoadBlock2.transform.position = tempRoadBlock1.transform.position + tempRoadBlock1.transform.forward * roadBlockWidth;
                tempRoadBlock2.transform.rotation = tempRoadBlock1.transform.rotation;
                tempRoadBlock2.transform.parent = gamePlaneOrigin.transform;

                roadBlockPlaced = true;
            }


        }
    }

    // Assume the road block is a child of gamePlaneOrigin.
    // TODO: could be removed
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

    //TODO: not sure if this works
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

        Debug.Log(positionsOK[0]);
        Debug.Log(positionsOK[1]);
        Debug.Log(positionsOK[2]);

        int choice = RandomChoose(positionsOK);

        PositionAndIndex posAndIdx = new PositionAndIndex(choice, localPositions[choice]);

        Debug.Log(posAndIdx.idx);

        return posAndIdx;
    }

    private int RandomChoose(bool[] positionsOK)
    {
        int index = rnd.Next(0, 3);
        while (!positionsOK[index])
        {
            index = rnd.Next(0, 3);
        }
        Debug.Log("outofloop");
        return index;
    }

    private Vector3 CalculateNextPosition(Vector3 currentPosition, Vector3 direction)
    {
        return currentPosition + direction * roadBlockWidth;
    }

    public void PlaceARoadBlock()
    {
        if (stepCount % 2 == 0)
        {
            PositionAndIndex newPosAndIdx = ChooseNextPos(tempRoadBlock2.transform);
            tempRoadBlock1.transform.localPosition = newPosAndIdx.pos;
            RotateAccordingToIndexAndOldBlock(tempRoadBlock1.transform, tempRoadBlock2.transform, newPosAndIdx.idx);
        }
        else
        {
            PositionAndIndex newPosAndIdx = ChooseNextPos(tempRoadBlock1.transform);
            tempRoadBlock2.transform.localPosition = newPosAndIdx.pos;
            RotateAccordingToIndexAndOldBlock(tempRoadBlock2.transform, tempRoadBlock1.transform, newPosAndIdx.idx);
        }
        stepCount++;
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

    public void StartGame()
    {
        gameStarted = true;
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