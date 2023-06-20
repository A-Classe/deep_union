using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDrawer : MonoBehaviour
{
    private DrawLine line;
    InputStateHistory history;

    void Awake()
    {
        line = GetComponent<DrawLine>();
        history = new InputStateHistory<Vector2>(Mouse.current.position);
    }

    void OnDestroy()
    {
        history.Dispose();
    }

    void OnEnable() => history.StartRecording();

    void OnDisable() => history.StopRecording();


    void Update()
    {
        foreach (InputStateHistory.Record record in history)
        {
            var pos = record.ReadValue<Vector2>();
            line.AddPosition(pos);
        }

        history.Clear();
    }
}