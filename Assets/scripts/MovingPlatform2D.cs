using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 moveDirection = Vector2.right; 
    public float moveDistance = 3f;              
    public float speed = 2f;                  
    public bool isMoving = false;

    Rigidbody2D rb;
    Vector2 startPos;
    Vector2 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = transform.position;
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        if (!isMoving) { return; }
        
        Vector2 targetPos = startPos + moveDirection.normalized * moveDistance;

        float t = Mathf.PingPong(Time.time * speed, 1f); 
        Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);

        rb.MovePosition(newPos);

        
        Velocity = (newPos - lastPos) / Time.fixedDeltaTime;

        lastPos = newPos;
    }

    public Vector2 Velocity { get; private set; }

    public void Activate()
    {
        transform.position = startPos;
        isMoving = true;
    }
    public void Deactivate() => isMoving = false;
    public void SetActiveState(bool state) => isMoving = state;
}
