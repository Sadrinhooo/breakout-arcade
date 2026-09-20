using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallTrajectory : MonoBehaviour
{
    public static BallTrajectory instance { get; private set; }

    [Header("Simulation")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private string padleTag = "Padle";
    [SerializeField] private int maxBounces = 6;
    [SerializeField] private float maxDistance = 40f;
    [SerializeField] private float skin = 0.02f;
    [SerializeField] private float arriveThreshold = 0.08f;

    [Header("Line")]
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private float lineZ;
    [SerializeField] private Material lineMaterial;

    private LineRenderer line;
    private CircleCollider2D ballCollider;
    private readonly List<Vector3> points = new List<Vector3>(16);
    private readonly RaycastHit2D[] hits = new RaycastHit2D[1];
    private Vector3[] drawBuffer = new Vector3[16];
    private ContactFilter2D filter;
    private bool hasPath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.loop = false;
        line.positionCount = 0;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.numCapVertices = 4;
        line.numCornerVertices = 2;
        line.enabled = false;

        if (lineMaterial != null)
            line.material = lineMaterial;

        filter.useTriggers = false;
        filter.SetLayerMask(collisionMask);
        filter.useLayerMask = true;
    }

    private void LateUpdate()
    {
        if (!hasPath || BallBehaviour.instance == null)
            return;

        ConsumeTravelled(BallBehaviour.instance.transform.position);

        if (points.Count < 2)
            Clear();
        else
            Draw();
    }

    public void Recalculate(Vector2 origin, Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            Clear();
            return;
        }

        if (ballCollider == null && BallBehaviour.instance != null)
            ballCollider = BallBehaviour.instance.GetComponent<CircleCollider2D>();

        Rebuild(origin, direction.normalized);
        Draw();
    }

    private void Rebuild(Vector2 origin, Vector2 direction)
    {
        points.Clear();
        points.Add(ToLinePoint(origin));

        float radius = GetBallRadius();
        float remaining = maxDistance;
        int bounces = 0;
        bool ignorePaddle = true;

        while (remaining > 0f && bounces <= maxBounces)
        {
            Vector2 castOrigin = origin + direction * skin;
            int hitCount = Physics2D.CircleCast(castOrigin, radius, direction, filter, hits, remaining);

            if (hitCount == 0)
            {
                points.Add(ToLinePoint(origin + direction * remaining));
                break;
            }

            RaycastHit2D hit = hits[0];
            Vector2 contactCenter = hit.centroid;

            if (ignorePaddle && hit.collider != null && hit.collider.CompareTag(padleTag))
            {
                origin = contactCenter + direction * skin;
                remaining -= hit.distance + skin;
                ignorePaddle = false;
                continue;
            }

            points.Add(ToLinePoint(contactCenter));
            remaining -= hit.distance + skin;
            origin = contactCenter + hit.normal * skin;
            direction = Vector2.Reflect(direction, hit.normal).normalized;
            ignorePaddle = false;
            bounces++;
        }

        hasPath = points.Count >= 2;
    }

    private void ConsumeTravelled(Vector2 ballPosition)
    {
        while (points.Count >= 2)
        {
            Vector2 from = points[0];
            Vector2 to = points[1];
            Vector2 segment = to - from;
            float segmentSqr = segment.sqrMagnitude;

            if (segmentSqr < 0.0001f)
            {
                points.RemoveAt(0);
                continue;
            }

            float t = Vector2.Dot(ballPosition - from, segment) / segmentSqr;
            bool reachedCorner = (ballPosition - to).sqrMagnitude <= arriveThreshold * arriveThreshold;

            if (t >= 1f || reachedCorner)
                points.RemoveAt(0);
            else
                break;
        }
    }

    private void Draw()
    {
        if (!hasPath || points.Count < 2)
        {
            Clear();
            return;
        }

        if (drawBuffer.Length < points.Count)
            drawBuffer = new Vector3[points.Count];

        for (int i = 0; i < points.Count; i++)
            drawBuffer[i] = points[i];

        line.enabled = true;
        line.positionCount = points.Count;
        line.SetPositions(drawBuffer);
    }

    public void Clear()
    {
        hasPath = false;
        points.Clear();
        line.positionCount = 0;
        line.enabled = false;
    }

    private float GetBallRadius()
    {
        if (ballCollider == null)
            return 0.1f;

        float scale = Mathf.Max(ballCollider.transform.lossyScale.x, ballCollider.transform.lossyScale.y);
        return ballCollider.radius * scale;
    }

    private Vector3 ToLinePoint(Vector2 point)
    {
        return new Vector3(point.x, point.y, lineZ);
    }
}
