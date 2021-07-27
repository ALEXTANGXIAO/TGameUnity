using System;
using UnityEngine;
using UnityEngine.UI;

public class FPS : UIWindow
{

}

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        public Text m_Text;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            //m_Text = GetComponent<Text>();
        }


        private void Update()
        {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;

                m_Text.color = m_CurrentFps >= 20 ? Color.white : (m_CurrentFps > 15 ? Color.yellow : Color.red);

                m_Text.text = m_CurrentFps.ToString();/*string.Format(display, m_CurrentFps)*/;
            }
        }
    }
}

