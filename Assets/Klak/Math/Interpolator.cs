//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using System;

namespace Klak.Math
{
    public struct FloatInterpolator
    {
        #region Nested Public Classes

        [Serializable]
        public class Config
        {
            public enum InterpolationType {
                Direct, Exponential, DampedSpring
            }

            [SerializeField]
            InterpolationType _interpolationType
                = InterpolationType.DampedSpring;

            public InterpolationType interpolationType {
                get { return _interpolationType; }
                set { _interpolationType = value; }
            }

            public bool enabled {
                get { return interpolationType != InterpolationType.Direct; }
            }

            [SerializeField, Range(0.1f, 100)]
            float _interpolationSpeed = 10;

            public float interpolationSpeed {
                get { return _interpolationSpeed; }
                set { _interpolationSpeed = value; }
            }

            public Config() {}

            public Config(InterpolationType type, float speed)
            {
                _interpolationType = type;
                _interpolationSpeed = speed;
            }

            public static Config Direct {
                get { return new Config(InterpolationType.Direct, 10); }
            }

            public static Config Quick {
                get { return new Config(InterpolationType.DampedSpring, 50); }
            }
        }

        #endregion

        #region Private Members

        float _velocity;

        #endregion

        #region Public Properties And Methods

        public Config config { get; set; }
        public float currentValue { get; set; }
        public float targetValue { get; set; }

        public FloatInterpolator(float initialValue, Config config)
        {
            this.config = config;
            currentValue = targetValue =initialValue;
            _velocity = 0;
        }

        public float Step(float targetValue)
        {
            this.targetValue = targetValue;
            return Step();
        }

        public float Step()
        {
            if (config.interpolationType == Config.InterpolationType.Exponential)
            {
                currentValue = ETween.Step(
                    currentValue, targetValue, config.interpolationSpeed);
            }
            else if (config.interpolationType == Config.InterpolationType.DampedSpring)
            {
                currentValue = DTween.Step(
                    currentValue, targetValue, ref _velocity, config.interpolationSpeed);
            }
            else
            {
                currentValue = targetValue;
            }
            return currentValue;
        }

        #endregion
    }
}
