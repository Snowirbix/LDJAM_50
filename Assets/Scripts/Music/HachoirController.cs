using PathCreation;
using UnityEngine;

public class HachoirController : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    public float speedForward = 100f;
    public float speedBackward = 50f;
    float distanceTravelled;
    public bool isPaused = true;
    public enum Direction
    {
        Forward,
        Backward,
        Stop
    }
    public Direction direction = Direction.Forward;

    private void Start()
    {
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
            //Debug.Log($"length{pathCreator.path.length}");
        }
    }

    public void Play()
    {
        isPaused = false;
        direction = Direction.Forward;
    }

    public void Pause()
    {
        isPaused = true;
    }

    private void Update()
    {
        if (pathCreator != null && !isPaused && direction != Direction.Stop)
        {
            if (direction == Direction.Forward)
            {
                distanceTravelled += speedForward * Time.deltaTime;
                if (distanceTravelled > pathCreator.path.length)
                {
                    direction = Direction.Backward;
                    distanceTravelled = 0f;
                    return;
                }
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
            else if (direction == Direction.Backward)
            {
                distanceTravelled += speedBackward * Time.deltaTime;
                if (distanceTravelled > pathCreator.path.length)
                {
                    Debug.Log(distanceTravelled);
                    direction = Direction.Stop;
                    distanceTravelled = 0f;
                    return;
                }
                transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length - distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length - distanceTravelled, endOfPathInstruction);
            }
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}