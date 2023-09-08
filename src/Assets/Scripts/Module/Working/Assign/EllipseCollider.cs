using System;
using Module.Working;
using UnityEngine;
using UnityEngine.Serialization;

public class EllipseCollider : MonoBehaviour
{
    [SerializeField] private float debugHeight = 1f;
    [SerializeField] private int debugResolution = 50;

    private Vector2 size;

    public event Action<Worker> OnWorkerEnter;
    public event Action<Worker> OnWorkerExit;

    public void SetSize(Vector2 size)
    {
        this.size = size;
    }

    public bool InEllipse(Vector3 point)
    {
        var position = transform.position;
        var dx = (point.x - position.x) / (size.x * 0.5f);
        var dz = (point.z - position.z) / (size.y * 0.5f);

        return (dx * dx + dz * dz) <= 1;
    }

    internal void OnWorkerEnter_Internal(Worker worker)
    {
        OnWorkerEnter?.Invoke(worker);
    }
    
    internal void OnWorkerExit_Internal(Worker worker)
    {
        OnWorkerExit?.Invoke(worker);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var angleStep = 360f / debugResolution;

        Vector3 prevPoint = Vector3.zero;

        for (var i = 0; i <= debugResolution; i++)
        {
            var angle = i * angleStep;
            var position = transform.position;
            var x = position.x + size.x * 0.5f * Mathf.Cos(angle * Mathf.Deg2Rad);
            var z = position.z + size.y * 0.5f * Mathf.Sin(angle * Mathf.Deg2Rad);
            var currentPoint = new Vector3(x, position.y + debugHeight, z);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, currentPoint);
            }

            prevPoint = currentPoint;
        }
    }
}