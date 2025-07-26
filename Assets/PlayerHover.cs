
using Unity.VisualScripting;
using UnityEngine;



public class PlayerHover : MonoBehaviour

{
    public float dampFactor = 0.5f;
    public float dampFrequency = 15; // 15
    public float hoverHeight = 1f;
    public float maxDistance = 3;
    public float castRadius = .5f;
    public Rigidbody rb;

    RaycastHit[] hits = new RaycastHit[10];

    private void Awake()
    {
        if (!rb)
        {
            Debug.LogError($"[{nameof(PlayerHover)}] missing field RigidBody.");
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!gameObject.GetComponent<PlayerGrind>().onRail)
        {
            ApplyHoverForce();
        }
    }

    void ApplyHoverForce()
    {
        if (GroundCast(out RaycastHit hit))
        {
            if (GetComponent<PlayerGrind>().onRail) // If the player is on a rail, and hits a wall, then the player bounces back.
            {
                // ThrowOffRail();
                gameObject.transform.RotateAround(transform.position, transform.right, 180f);
                GetComponent<PlayerGrind>().CalculateAndSetRailPosition();
            }
            if (hit.transform.gameObject.CompareTag("Rail")) // If the player hits a rail, then the player rides the rail.
            {
                GetComponent<PlayerGrind>().RideRail(hit.transform.gameObject);
            }
            else // The player is not on a rail or hitting a rail, so apply hover force normally.
            {
                Vector3 rayDirection = Vector3.down;
                float springDelta = GetSpringDelta(hit);
                float springStrength = SpringStrength(rb.mass, dampFrequency);
                float dampStrength = DampStrength(dampFactor, rb.mass, dampFrequency);
                float springSpeed = GetRelativeSpeedAlongDirection(rb, hit.rigidbody, rayDirection);
                Vector3 springForce = GetSpringForce(
                    springDelta,
                    springSpeed,
                    springStrength,
                    dampStrength,
                    rayDirection);
                springForce -= Physics.gravity;
                rb.AddForce(springForce);
                if (hit.rigidbody) hit.rigidbody.AddForceAtPosition(-springForce, hit.point);
            }
        }
    }

    bool GroundCast(out RaycastHit hit)
    {
        int hitCount = Physics.SphereCastNonAlloc(
            transform.position,
            castRadius,
            -transform.up,
            hits,
            maxDistance);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit current = hits[i];
                if (current.rigidbody == rb) continue;
                hit = current;
                return true;
            }
        }
        hit = default;
        return false;
    }

    float GetSpringDelta(RaycastHit hit)
    {
        return hit.distance - (hoverHeight - castRadius);
    }

    static float GetRelativeSpeedAlongDirection(
        Rigidbody targetBody,
        Rigidbody frameBody,
        Vector3 direction)
    {
        Vector3 velocity = targetBody.linearVelocity;
        Vector3 hitBodyVelocity = frameBody ? frameBody.linearVelocity : default;
        float rayDirectionSpeed = Vector3.Dot(direction, velocity);
        float hitBodyRayDirectionSpeed = Vector3.Dot(direction, hitBodyVelocity);
        return rayDirectionSpeed - hitBodyRayDirectionSpeed;
    }

    static float SpringStrength(float mass, float frequency)
    {
        return frequency * frequency * mass;
    }

    static float DampStrength(float dampFactor, float mass, float frequency)
    {
        float criticalDampStrength = 2 * mass * frequency;
        return dampFactor * criticalDampStrength;
    }

    static Vector3 GetSpringForce(
        float springDelta,
        float springSpeed,
        float springStrength,
        float dampStrength,
        Vector3 direction)
    {
        float tension = springDelta * springStrength;
        float damp = springSpeed * dampStrength;
        float forceMagnitude = tension - damp;
        Vector3 force = direction * forceMagnitude;
        return force;
    }
}