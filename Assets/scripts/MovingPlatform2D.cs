using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    [Header("Path")]
    public Vector2 moveDirection = Vector2.right;
    public float moveDistance = 3f;

    [Header("Motion Profile")]
    public float maxSpeed = 2f;          // units/sec
    public float acceleration = 6f;      // units/sec^2
    public float decelDistance = 0.5f;   // units before end to start braking

    [Header("State")]
    public bool isMoving = false;

    public Vector2 Velocity { get; private set; }

    Rigidbody2D rb;

    Vector2 startPos;
    Vector2 endPos;
    Vector2 lastPos;

    float speed;        // current speed magnitude along the path
    int dirSign = 1;    // +1 going to endPos, -1 going to startPos


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = rb.position;
        lastPos = rb.position;
        endPos = startPos + moveDirection.normalized * moveDistance;

        Velocity = Vector2.zero;
        speed = 0f;
        dirSign = 1;
    }

    void FixedUpdate()
    {
        if (!isMoving) 
        {
            Velocity = Vector2.zero;
            lastPos = rb.position;
            return; 
        }

        // Recompute end in case you tweak values in inspector
        Vector2 dir = moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector2.right;
        endPos = startPos + dir * moveDistance;

        Vector2 current = rb.position;
        Vector2 target = (dirSign > 0) ? endPos : startPos;

        float distToTarget = Vector2.Distance(current, target);

        // --- Decide whether to accelerate or decelerate ---
        // braking when close to endpoint
        bool shouldBrake = distToTarget <= Mathf.Max(0.001f, decelDistance);

        if (shouldBrake)
        {
            // decelerate toward 0
            speed = Mathf.MoveTowards(speed, 0f, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // accelerate toward maxSpeed
            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.fixedDeltaTime);
        }

        // --- Move ---
        Vector2 moveStepDir = (target - current);
        if (moveStepDir.sqrMagnitude > 0.000001f)
            moveStepDir.Normalize();
        else
            moveStepDir = dir * dirSign;

        float step = speed * Time.fixedDeltaTime;

        // Don't overshoot
        if (step > distToTarget) step = distToTarget;

        Vector2 newPos = current + moveStepDir * step;
        rb.MovePosition(newPos);

        // stable velocity for player carry
        Velocity = (newPos - lastPos) / Time.fixedDeltaTime;
        lastPos = newPos;

        // --- Turnaround at the endpoint ---
        // We flip direction only after we actually "arrive" and slowed down
        if (distToTarget <= 0.001f && speed <= 0.001f)
        {
            dirSign *= -1;
        }
    }


    public void Activate()
    {

        // Start from wherever the platform currently is
        startPos = rb.position;
        Vector2 dir = moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector2.right;
        endPos = startPos + dir * moveDistance;

        speed = 0f;
        dirSign = 1;
        lastPos = rb.position;
        Velocity = Vector2.zero;

        isMoving = true;
    }
    public void Deactivate()
    {
        isMoving = false;
        speed = 0f;
        Velocity = Vector2.zero;
    }
    public void SetActiveState(bool state)
    {
        if (state) Activate();
        else Deactivate();
    }
}
