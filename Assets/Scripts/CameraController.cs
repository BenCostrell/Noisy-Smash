using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float baseSize;
    public float movementSpeed;
    public float minSizeFactor;
    public float maxSizeFactor;
    public float playerDistToCameraSizeRatio;
    public float sizeAdjustSpeed;
    public Vector2 bottomLeftCorner_MinCameraPos;
    public Vector2 topRightCorner_MaxCameraPos;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Services.GameManager.fightActive)
        {
            FollowPlayers();
        }
    }

    void FollowPlayers()
    {
        Vector3 player1Pos = Services.GameManager.players[0].transform.position;
        Vector3 player2Pos = Services.GameManager.players[1].transform.position;
        Vector3 playerMidpoint = (player1Pos + player2Pos) / 2;
        float playerDistance = Vector3.Distance(player1Pos, player2Pos);
        float targetXPos = Mathf.Clamp(playerMidpoint.x, bottomLeftCorner_MinCameraPos.x, topRightCorner_MaxCameraPos.x);
        float targetYPos = Mathf.Clamp(playerMidpoint.y, bottomLeftCorner_MinCameraPos.y, topRightCorner_MaxCameraPos.y);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetXPos, targetYPos, transform.position.z),
            movementSpeed * Time.deltaTime);
        float sizeFromPlayerDist = playerDistance * playerDistToCameraSizeRatio;
        float targetSize = Mathf.Clamp(sizeFromPlayerDist, minSizeFactor * baseSize, maxSizeFactor * baseSize);
        float currentSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize += (targetSize - currentSize) * sizeAdjustSpeed;
    }

    public void ResetCamera()
    {
        Services.GameManager.fightActive = false;
        transform.position = new Vector3(0, 0, transform.position.z);
        Camera.main.orthographicSize = baseSize;
    }

    public void ResumeCameraFollow()
    {
        Services.GameManager.fightActive = true;
    }
}
