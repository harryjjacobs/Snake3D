using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Turn
{
    public Vector3 point;
    public float angle;

    public Turn(Vector3 point, float angle)
    {
        this.point = point;
        this.angle = angle;
    }
}

public class SnakeController : MonoBehaviour {

    public float moveRate = 1f;
    public float growDelay = 5f;
    public float segmentGapSize = 0.2f;
    public LayerMask ground;
    public GameObject segmentPrefab;

    private float moveCounter = 0;
    private bool horizontalReleased;
    private bool disableTurn;
    private bool moveForwardFirst;
    private List<Segment> segments;
    private bool newSegmentAdded;
    private bool isDead;

    void Awake()
    {
        AudioManager.RegisterAudioSource("snake", GetComponent<AudioSource>());
    }

    void OnEnable()
    {
        isDead = false;
        if (segments != null) segments.Clear();
        segments = new List<Segment>();
        for (int i = 0; i < transform.childCount; i++)
        {
            segments.Add(new Segment(transform.GetChild(i)));
        }
        InvokeRepeating("AddSegment", growDelay, growDelay);
        Camera.main.GetComponent<SmoothFollow>().target = segments[0].transform;
        Camera.main.GetComponent<SmoothFollow>().JumpToTarget();
    }

    void Update () {

        if (SegmentsOverlapping() && !isDead) DoDeath();

        float hInput = Input.GetAxis("Horizontal");

#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            hInput = (Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position).x - 0.5f);
        }
#endif

        if (hInput == 0)
        {
            horizontalReleased = true;
        }
        //Do turning
        else if (horizontalReleased && !disableTurn)
        {
            horizontalReleased = false;

            if (moveForwardFirst)
            {
                DoTurns();
                MoveAllSegmentsForward();
                moveForwardFirst = false;
            }

            int direction = (int) Mathf.Clamp(hInput * 1000, -1, 1);
            float angle = 90 * direction;
            Vector3 position = segments[0].transform.position;
            Turn newTurn = new Turn(position, angle);
            AddTurnAtPoint(newTurn);
            DoTurns();
            MoveAllSegmentsForward();
            moveCounter = 0;

        }

        //Do movement and turning
        if (moveCounter >= 1 / moveRate)
        {
            DoTurns();
            MoveAllSegmentsForward();
            moveCounter = 0;
        }
        moveCounter += Time.deltaTime;
    }

    void AddTurnAtPoint(Turn turn)
    {
        int cnt = segments.Count;
        newSegmentAdded = false;
        for (int i = 0; i < cnt; i++)
        {
            if (newSegmentAdded)
            {
                cnt += 1;
                newSegmentAdded = false;
            }
            segments[i].turnPoints.Enqueue(turn);
        }
    }

    void DoTurns()
    {
        int cnt = segments.Count;
        newSegmentAdded = false;
        for (int i = 0; i < cnt; i++)
        {
            if (newSegmentAdded)
            {
                cnt += 1;
                newSegmentAdded = false;
            }
            TryNextTurn(segments[i]);
        }
    }

    void MoveAllSegmentsForward()
    {
        int cnt = segments.Count;
        newSegmentAdded = false;
        for (int i = 0; i < cnt; i++)
        {
            if (newSegmentAdded)
            {
                cnt += 1;
                newSegmentAdded = false;
            }
            MoveSegmentForward(segments[i], i == 0);
        }
    }

    void MoveSegmentForward(Segment segment, bool isFirstSegment)
    {
        if (!segment.transform) return;
        segment.transform.position += segment.transform.forward;
        segment.transform.position = Utilities.RoundToNearest(segment.transform.position, 1);

        bool isAboveGround = IsThereGround(segment.transform.position, -segment.transform.up);
        if (!isAboveGround) segment.transform.Rotate(segment.transform.right, 90f, Space.World);

        if (isFirstSegment)
        {
            if (!isAboveGround)
            {
                moveForwardFirst = true;
            } else
            {
                moveForwardFirst = false;
            }

            //We are in a tunnel so cannot turn
            if (IsThereGround(segment.transform.position, segment.transform.right) ||
                IsThereGround(segment.transform.position, -segment.transform.right))
            {
                disableTurn = true;
            } else
            {
                disableTurn = false;
            }
        }
    }

    void TryNextTurn(Segment segment)
    {
        if (segment.turnPoints.Count == 0 || !segment.transform) return;
        Turn turn = segment.turnPoints.Peek();
        //Debug.Log(segment.transform.position);
        //Debug.Log(turn.point);
        if (segment.transform.position == turn.point)
        {
            segment.turnPoints.Dequeue();
            //Debug.Log("Dequeued " + turn.point);
            segment.transform.Rotate(segment.transform.up, turn.angle, Space.World);
            //Avoid rounding errors
            segment.transform.rotation = Quaternion.Euler(Mathf.RoundToInt(segment.transform.rotation.eulerAngles.x), Mathf.RoundToInt(segment.transform.rotation.eulerAngles.y), Mathf.RoundToInt(segment.transform.rotation.eulerAngles.z));
        }
    }

    void AddSegment()
    {
        Transform lastSegment = segments[segments.Count - 1].transform;
        if (!lastSegment) return;
        Vector3 spawnPosition = lastSegment.position - lastSegment.forward;
        Quaternion rotation = lastSegment.rotation;
        Transform newTransform = Instantiate(segmentPrefab, spawnPosition, rotation, transform).transform;

        //Check if the previous last segment is off the edge
        Ray ray = new Ray(lastSegment.position, -lastSegment.up);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1f, ground))
        {
            newTransform.position -= newTransform.up;
            newTransform.position += newTransform.forward;
            newTransform.Rotate(newTransform.right, -90f, Space.World);
        }

        Segment newSegment = new Segment(newTransform);
        Queue<Turn> newQueue = new Queue<Turn>(segments[segments.Count - 1].turnPoints);
        newSegment.turnPoints = newQueue;
        segments.Add(newSegment);

        newSegmentAdded = true;

        if (segments.Count >= GameManager.maxScore)
        {
            GameManager.PlayerWon();
        }

    }


    bool IsThereGround(Vector3 position, Vector3 direction)
    {
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f, ground))
        {
            return true;
        }
        return false;
    }


    bool SegmentsOverlapping()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < segments.Count; j++)
            {
                if (i == j) continue;
                if (segments[i].transform.GetComponent<Collider>().bounds.Intersects(segments[j].transform.GetComponent<Collider>().bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DoDeath()
    {
        isDead = true;
        DoTurns();
        MoveAllSegmentsForward();
        CancelInvoke();
        GameManager.PlayerDied();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            DoDeath();
        }
        else if (col.tag == "Food")
        {
            //play sound
            AudioManager.PlaySound("snake", "food_pickup");
            Destroy(col.gameObject);
            AddSegment();
            UI.UpdateScore(segments.Count, GameManager.maxScore);
            GameManager.Instance.FoodSpawn();
        }
    }

}
