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

namespace Klak.Motion
{
    [AddComponentMenu("Klak/Motion/Constant Motion")]
    public class ConstantMotion : MonoBehaviour
    {
        #region Nested Classes

        public enum TranslationMode {
            Off, XAxis, YAxis, ZAxis, Vector, Random
        };

        public enum RotationMode {
            Off, XAxis, YAxis, ZAxis, Vector, Random
        }

        #endregion

        #region Editable Properties

        [SerializeField]
        TranslationMode _translationMode = TranslationMode.Off;

        [SerializeField]
        Vector3 _translationVector = Vector3.forward;

        [SerializeField]
        float _translationSpeed = 1.0f;

        [SerializeField]
        RotationMode _rotationMode = RotationMode.Off;

        [SerializeField]
        Vector3 _rotationAxis = Vector3.up;

        [SerializeField]
        float _rotationSpeed = 30.0f;

        [SerializeField]
        bool _useLocalCoordinate = true;

        #endregion

        #region Public Properties

        public TranslationMode translationMode {
            get { return _translationMode; }
            set { _translationMode = value; }
        }

        public Vector3 translationVector {
            get { return _translationVector; }
            set { _translationVector = value; }
        }

        public float translationSpeed {
            get { return _translationSpeed; }
            set { _translationSpeed = value; }
        }

        public RotationMode rotationMode {
            get { return _rotationMode; }
            set { _rotationMode = value; }
        }

        public Vector3 rotationAxis {
            get { return _rotationAxis; }
            set { _rotationAxis = value; }
        }

        public float rotationSpeed {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }

        public bool useLocalCoordinate {
            get { return _useLocalCoordinate; }
            set { _useLocalCoordinate = value; }
        }

        #endregion

        #region Private Variables

        Vector3 _randomVectorT;
        Vector3 _randomVectorR;

        Vector3 TranslationVector {
            get {
                switch (_translationMode)
                {
                    case TranslationMode.XAxis:  return Vector3.right;
                    case TranslationMode.YAxis:  return Vector3.up;
                    case TranslationMode.ZAxis:  return Vector3.forward;
                    case TranslationMode.Vector: return _translationVector;
                }
                // TranslationMode.Random
                return _randomVectorT;
            }
        }

        Vector3 RotationVector {
            get {
                switch (_rotationMode)
                {
                    case RotationMode.XAxis:  return Vector3.right;
                    case RotationMode.YAxis:  return Vector3.up;
                    case RotationMode.ZAxis:  return Vector3.forward;
                    case RotationMode.Vector: return _rotationAxis;
                }
                // RotationMode.Random
                return _randomVectorR;
            }
        }

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _randomVectorT = Random.onUnitSphere;
            _randomVectorR = Random.onUnitSphere;
        }

        void Update()
        {
            var dt = Time.deltaTime;

            if (_translationMode != TranslationMode.Off)
            {
                var dp = TranslationVector * _translationSpeed * dt;

                if (_useLocalCoordinate)
                    transform.localPosition += dp;
                else
                    transform.position += dp;
            }

            if (_rotationMode != RotationMode.Off)
            {
                var dr = Quaternion.AngleAxis(
                    _rotationSpeed * dt, RotationVector);

                if (_useLocalCoordinate)
                    transform.localRotation = dr * transform.localRotation;
                else
                    transform.rotation = dr * transform.rotation;
            }
        }

        #endregion
    }
}
