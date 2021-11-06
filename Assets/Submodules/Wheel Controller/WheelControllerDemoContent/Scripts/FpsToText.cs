using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// Source: https://forum.unity.com/threads/fpstotext-free-fps-framerate-calculator-with-options.463667/
namespace Performance
{
    /// <summary>
    ///     <para>Pushes the Framerate value to a Text component.</para>
    /// </summary>
    public class FpsToText : MonoBehaviour
    {
        public Color Bad            = Color.red;
        public int   BadBelow       = 30;
        public bool  ForceIntResult = true;
        public Color Good           = Color.green;

        [Header("// Sample Groups of Data ")]
        public bool GroupSampling = true;

        public int   MaxTextLength = 5;
        public Color Okay          = Color.yellow;
        public int   OkayBelow     = 60;
        public int   SampleSize    = 20;
        public bool  Smoothed      = true;

        [Header("// Config ")]
        public Text TargetText;

        public int UpdateTextEvery = 1;

        [Header("// Color Config ")]
        public bool UseColors = true;

        [Header("// System FPS (updates once/sec)")]
        public bool UseSystemTick;

        protected float[] FpsSamples;
        protected int     SampleIndex;
        protected int     TextUpdateIndex;
        private   int     _sysFrameRate;
        private   int     _sysLastFrameRate;

        private int _sysLastSysTick;

        public float Framerate { get; private set; }


        protected virtual void Start()
        {
            FpsSamples = new float[SampleSize];
            for (int i = 0; i < FpsSamples.Length; i++)
            {
                FpsSamples[i] = 0.001f;
            }

            if (!TargetText)
            {
                enabled = false;
            }
        }


        protected virtual void Update()
        {
            if (GroupSampling)
            {
                Group();
            }
            else
            {
                SingleFrame();
            }

            string fps = Framerate.ToString(CultureInfo.CurrentCulture);

            SampleIndex     = SampleIndex < SampleSize - 1 ? SampleIndex + 1 : 0;
            TextUpdateIndex = TextUpdateIndex > UpdateTextEvery ? 0 : TextUpdateIndex + 1;
            if (TextUpdateIndex == UpdateTextEvery)
            {
                TargetText.text = fps.Substring(0, fps.Length < 5 ? fps.Length : 5);
            }

            if (!UseColors)
            {
                return;
            }

            if (Framerate < BadBelow)
            {
                TargetText.color = Bad;
                return;
            }

            TargetText.color = Framerate < OkayBelow ? Okay : Good;
        }


        protected virtual int GetSystemFramerate()
        {
            if (Environment.TickCount - _sysLastSysTick >= 1000)
            {
                _sysLastFrameRate = _sysFrameRate;
                _sysFrameRate     = 0;
                _sysLastSysTick   = Environment.TickCount;
            }

            _sysFrameRate++;
            return _sysLastFrameRate;
        }


        protected virtual void Group()
        {
            FpsSamples[SampleIndex] = UseSystemTick
                                          ? GetSystemFramerate()
                                          : Smoothed
                                              ? 1 / Time.smoothDeltaTime
                                              : 1 / Time.deltaTime;

            Framerate = 0;
            bool loop = true;
            int  i    = 0;
            while (loop)
            {
                if (i == SampleSize - 1)
                {
                    loop = false;
                }

                Framerate += FpsSamples[i];
                i++;
            }

            Framerate /= FpsSamples.Length;
            if (ForceIntResult)
            {
                Framerate = (int) Framerate;
            }
        }


        protected virtual void Reset()
        {
            SampleSize      = 20;
            UpdateTextEvery = 1;
            MaxTextLength   = 5;
            Smoothed        = true;
            UseColors       = true;
            Good            = Color.green;
            Okay            = Color.yellow;
            Bad             = Color.red;
            OkayBelow       = 60;
            BadBelow        = 30;
            UseSystemTick   = false;
            ForceIntResult  = true;
        }


        protected virtual void SingleFrame()
        {
            Framerate = UseSystemTick
                            ? GetSystemFramerate()
                            : Smoothed
                                ? 1 / Time.smoothDeltaTime
                                : 1 / Time.deltaTime;
            if (ForceIntResult)
            {
                Framerate = (int) Framerate;
            }
        }
    }
}