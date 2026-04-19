using Fusion;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;

public class LatencyDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] bool latencyDisplayTest;
    public void UpdateLatency(int ms)
    {
        text.text = "Latency: " + ms.ToString() + "ms";
    }

    private NetworkRunner _runner;

    private void Update()
    {
        if (_runner == null)
        {
            _runner = FindFirstObjectByType<NetworkRunner>();
            return;
        }

        if (_runner.IsRunning)
        {
            double ping = _runner.GetPlayerRtt(_runner.LocalPlayer) * 1000;
            text.text = $"Latency: {ping:F0} ms";
        }
    }
}
