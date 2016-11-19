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
using Klak.Math;
using Klak.VectorMathExtension;

namespace Klak.Motion
{
    /// Follows a given transform smoothly
    [AddComponentMenu("Klak/Motion/Smooth Follow")]
    public class SmoothFollow : MonoBehaviour
    {
        #region Nested Classes

        public enum Interpolator {
            Exponential, Spring, DampedSpring
        }

        #endregion

        #region Editable Properties

        [SerializeField]
        Interpolator _interpolator = Interpolator.DampedSpring;

        [SerializeField]
        Transform _target;

        [SerializeField, Range(0, 20)]
        float _positionSpeed = 2;

        [SerializeField, Range(0, 20)]
        float _rotationSpeed = 2;

        [SerializeField]
        float _jumpDistance = 1;

        [SerializeField, Range(0, 360)]
        float _jumpAngle = 60;

        #endregion

        #region Public Properties And Methods

        public Interpolator interpolationType {
            get { return _interpolator; }
            set { _interpolator = value; }
        }

        public Transform target {
            get { return _target; }
            set { _target = value; }
        }

        public float positionSpeed {
            get { return _positionSpeed; }
            set { _positionSpeed = value; }
        }

        public float rotationSpeed {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }

        public float jumpDistance {
            get { return _jumpDistance; }
            set { _jumpDistance = value; }
        }

        public float jumpAngle {
            get { return _jumpAngle; }
            set { _jumpAngle = value; }
        }

        public void Snap()
        {
            if (_positionSpeed > 0) transform.position = target.position;
            if (_rotationSpeed > 0) transform.rotation = target.rotation;
        }

        public void JumpRandomly()
        {
            var r1 = Random.Range(0.5f, 1.0f);
            var r2 = Random.Range(0.5f, 1.0f);

            var dp = Random.onUnitSphere * _jumpDistance * r1;
            var dr = Quaternion.AngleAxis(_jumpAngle * r2, Random.onUnitSphere);

            transform.position = dp + target.position;
            transform.rotation = dr * target.rotation;
        }

        #endregion

        #region Private Properties And Functions

        Vector3 _vposition;
        Vector4 _vrotation;

        Vector3 SpringPosition(Vector3 current, Vector3 target)
        {
            _vposition = ETween.Step(_vposition, Vector3.zero, 1 + _positionSpeed * 0.5f);
            _vposition += (target - current) * (_positionSpeed * 0.1f);
            return current + _vposition * Time.deltaTime;
        }

        Quaternion SpringRotation(Quaternion current, Quaternion target)
        {
            var v_current = current.ToVector4();
            var v_target = target.ToVector4();
            _vrotation = ETween.Step(_vrotation, Vector4.zero, 1 + _rotationSpeed * 0.5f);
            _vrotation += (v_target - v_current) * (_rotationSpeed * 0.1f);
            return (v_current + _vrotation * Time.deltaTime).ToNormalizedQuaternion();
        }

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            if (_interpolator == Interpolator.Exponential)
            {
                if (_positionSpeed > 0)
                    transform.position = ETween.Step(transform.position, target.position, _positionSpeed);
                if (_rotationSpeed > 0)
                    transform.rotation = ETween.Step(transform.rotation, target.rotation, _rotationSpeed);
            }
            else if (_interpolator == Interpolator.DampedSpring)
            {
                if (_positionSpeed > 0)
                    transform.position = DTween.Step(transform.position, target.position, ref _vposition, _positionSpeed);
                if (_rotationSpeed > 0)
                    transform.rotation = DTween.Step(transform.rotation, target.rotation, ref _vrotation, _rotationSpeed);
            }
            else
            {
                if (_positionSpeed > 0)
                    transform.position = SpringPosition(transform.position, target.position);
                if (_rotationSpeed > 0)
                    transform.rotation = SpringRotation(transform.rotation, target.rotation);
            }
        }

        #endregion
    }
}
