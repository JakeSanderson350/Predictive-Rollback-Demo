using TMPro;
using UnityEngine;

public class LatencyDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] bool latencyDisplayTest;
    public void UpdateLatency(int ms)
    {
        text.text = "Latency: " + ms.ToString() + "ms";
    }

    private void Update()
    {
        if (latencyDisplayTest)
        {
            int ms = (int)(Time.deltaTime * 10000f);
            UpdateLatency(ms);
        }

    }
}
