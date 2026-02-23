using UnityEngine;

public class BossFacing : MonoBehaviour
{
    [SerializeField] Transform spriteRoot;   
    [SerializeField] bool flipByScale = true;
    [SerializeField] float facingDeadzone = 0.05f;

    float baseXScale;

    void Awake()
    {
        if (spriteRoot == null) spriteRoot = transform;
        baseXScale = Mathf.Abs(spriteRoot.localScale.x);
    }

    public void FaceTarget(Vector3 targetPos)
    {
        float dx = targetPos.x - transform.position.x;
        if (Mathf.Abs(dx) < facingDeadzone) return;

        bool faceRight = dx > 0f;

        if (flipByScale)
        {
            Vector3 s = spriteRoot.localScale;
            s.x = (faceRight ? -baseXScale : +baseXScale);
            spriteRoot.localScale = s;
        }
        else
        {
            var sr = spriteRoot.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.flipX = !faceRight; 
        }
    }

    public bool IsFacingRight()
    {
        return spriteRoot.localScale.x < 0f;
    }
}
