using UnityEngine;
using UnityEngine.UI;

public class FPSShow : MonoBehaviour
{

    public Text FPSTex;

    private float time, frameCount;

    private float fps = 0;

    void Update()
    {

        if (time >= 1 && frameCount >= 1)

        {

            fps = frameCount / time;

            time = 0;

            frameCount = 0;

        }

        FPSTex.color = fps >= 20 ? Color.white : (fps > 15 ? Color.yellow : Color.red);

        FPSTex.text = "FPS为:" + fps.ToString("f2");

        time += Time.unscaledDeltaTime;

        frameCount++;

    }
}