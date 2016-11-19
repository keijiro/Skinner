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

namespace Klak.VectorMathExtension
{
    /// Extension methods for Vector4
    static class Vector4Extension
    {
        public static Quaternion ToQuaternion(this Vector4 v)
        {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        public static Quaternion ToNormalizedQuaternion(this Vector4 v)
        {
            v = Vector4.Normalize(v);
            return new Quaternion(v.x, v.y, v.z, v.w);
        }
    }

    /// Extension methods for Quaternion
    static class QuaternionExtension
    {
        public static Vector4 ToVector4(this Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }
    }
}
