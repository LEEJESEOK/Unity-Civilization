using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lightbug.CharacterControllerPro.Demo
{


    public class FpsCounter : MonoBehaviour
    {

        [SerializeField]
        float time = 1;

        [SerializeField]
        Text text = null;

        [Tooltip("If this is not true the text will include \"FPS : \" before the actual fps value")]
        [SerializeField]
        bool showOnlyNumbers = true;

        [SerializeField]
        bool limitToRefreshRate = true;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        float result = 0;
        int samples = 0;

        string output = "FPS : ";

        float fps = 60f;
        public float Fps
        {
            get
            {
                return fps;
            }
        }

        GUIStyle style = new GUIStyle();


        void Awake()
        {
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            fps = Screen.currentResolution.refreshRate;
        }

        void Update()
        {

            if (time > 0)
            {
                result += 1f / Time.unscaledDeltaTime;
                samples++;
                time -= Time.unscaledDeltaTime;
            }
            else
            {
                fps = result / samples;

                if (limitToRefreshRate && QualitySettings.vSyncCount != 0)
                    fps = Mathf.Min(fps, Screen.currentResolution.refreshRate);

                //  


                output = showOnlyNumbers ? fps.ToString("F1") : "FPS : " + fps.ToString("F1");

                if (text != null)
                    text.text = output;



                result = 0;
                samples = 0;
                time = 1;
            }

        }


    }

}
