using UnityEngine;

public class Attack : MonoBehaviour
{
    public int _attackStrength = 0;

    [SerializeField] private Transform _attackHitboxPoint; // Point from which the attack hitbox is checked
    [SerializeField] private LayerMask _enemyLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnableAttackHitbox()
    {
        // Detect collisions with enemies here using a capsule collider
        DrawDebugCapsule2D(
            _attackHitboxPoint.position,
            _attackHitboxPoint.localScale,
            CapsuleDirection2D.Horizontal,
            _attackHitboxPoint.rotation.eulerAngles.z,
            Color.red,
            0.1f);
        Collider2D[] enemyColliders = Physics2D.OverlapCapsuleAll(
            _attackHitboxPoint.position,
            _attackHitboxPoint.localScale,
            CapsuleDirection2D.Horizontal,
            _attackHitboxPoint.rotation.eulerAngles.z,
            _enemyLayer);

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            // Print out the name of the enemy hit for debugging purposes
            Debug.Log($"Hit enemy: {enemyCollider.name}");
            // get enemy health component and apply damage
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_attackStrength);
            }
        }
    }

    public static void DrawDebugCapsule2D(Vector2 center, Vector2 size, CapsuleDirection2D direction, float angleDegree, Color color, float duration = 0.04f)
    {
        float angleRad = angleDegree * Mathf.Deg2Rad;
        float width = size.x;
        float height = size.y;

        // Calculate core dimensions based on orientation
        float radius = (direction == CapsuleDirection2D.Vertical) ? width / 2f : height / 2f;
        float halfHeight = height / 2f;
        float halfWidth = width / 2f;

        // Local positions for the straight side line segments
        Vector2 localTopLeft, localTopRight, localBotLeft, localBotRight;

        if (direction == CapsuleDirection2D.Vertical)
        {
            float offset = halfHeight - radius;
            localTopLeft = new Vector2(-radius, offset);
            localTopRight = new Vector2(radius, offset);
            localBotLeft = new Vector2(-radius, -offset);
            localBotRight = new Vector2(radius, -offset);
        }
        else // Horizontal capsule orientation
        {
            float offset = halfWidth - radius;
            localTopLeft = new Vector2(-offset, radius);
            localTopRight = new Vector2(offset, radius);
            localBotLeft = new Vector2(-offset, -radius);
            localBotRight = new Vector2(offset, -radius);
        }

        // Matrix helper function to transform local positions into rotated world positions
        Vector2 TransformPoint(Vector2 localPoint)
        {
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);
            float tx = localPoint.x * cos - localPoint.y * sin;
            float ty = localPoint.x * sin + localPoint.y * cos;
            return center + new Vector2(tx, ty);
        }

        // Convert core structural anchor bounds to world points
        Vector2 wTL = TransformPoint(localTopLeft);
        Vector2 wTR = TransformPoint(localTopRight);
        Vector2 wBL = TransformPoint(localBotLeft);
        Vector2 wBR = TransformPoint(localBotRight);

        // Draw the straight capsule edges
        Debug.DrawLine(wTL, wBL, color, duration);
        Debug.DrawLine(wTR, wBR, color, duration);

        // Draw the semi-circular end caps dynamically using points
        int segments = 8;
        Vector2 prevCapStart = (direction == CapsuleDirection2D.Vertical) ? wTR : wTR;

        // Draw Top/Right Cap
        float startAngle = (direction == CapsuleDirection2D.Vertical) ? 0f : -90f;
        Vector2 capCenter = (direction == CapsuleDirection2D.Vertical) ? TransformPoint(new Vector2(0, halfHeight - radius)) : TransformPoint(new Vector2(halfWidth - radius, 0));

        for (int i = 0; i <= segments; i++)
        {
            float a = angleDegree + startAngle + (i * (180f / segments));
            float rad = a * Mathf.Deg2Rad;
            Vector2 nextPoint = capCenter + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            if (i > 0) Debug.DrawLine(prevCapStart, nextPoint, color, duration);
            prevCapStart = nextPoint;
        }

        // Draw Bottom/Left Cap
        startAngle = (direction == CapsuleDirection2D.Vertical) ? 180f : 90f;
        capCenter = (direction == CapsuleDirection2D.Vertical) ? TransformPoint(new Vector2(0, -halfHeight + radius)) : TransformPoint(new Vector2(-halfWidth + radius, 0));
        prevCapStart = (direction == CapsuleDirection2D.Vertical) ? wBL : wBL;

        for (int i = 0; i <= segments; i++)
        {
            float a = angleDegree + startAngle + (i * (180f / segments));
            float rad = a * Mathf.Deg2Rad;
            Vector2 nextPoint = capCenter + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            if (i > 0) Debug.DrawLine(prevCapStart, nextPoint, color, duration);
            prevCapStart = nextPoint;
        }
    }

}
