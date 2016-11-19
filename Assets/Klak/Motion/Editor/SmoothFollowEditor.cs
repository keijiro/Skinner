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
using UnityEditor;

namespace Klak.Motion
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SmoothFollow))]
    public class SmoothFollowEditor : Editor
    {
        SerializedProperty _interpolator;
        SerializedProperty _target;
        SerializedProperty _positionSpeed;
        SerializedProperty _rotationSpeed;
        SerializedProperty _jumpDistance;
        SerializedProperty _jumpAngle;

        static GUIContent _textAngle = new GUIContent("Angle");
        static GUIContent _textDistance = new GUIContent("Distance");
        static GUIContent _textPosition = new GUIContent("Position");
        static GUIContent _textRotation = new GUIContent("Rotation");

        void OnEnable()
        {
            _interpolator = serializedObject.FindProperty("_interpolator");
            _target = serializedObject.FindProperty("_target");
            _positionSpeed = serializedObject.FindProperty("_positionSpeed");
            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
            _jumpDistance = serializedObject.FindProperty("_jumpDistance");
            _jumpAngle = serializedObject.FindProperty("_jumpAngle");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_interpolator);
            EditorGUILayout.PropertyField(_target);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Interpolation Speed", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_positionSpeed, _textPosition);
            EditorGUILayout.PropertyField(_rotationSpeed, _textRotation);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Random Jump", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_jumpDistance, _textDistance);
            EditorGUILayout.PropertyField(_jumpAngle, _textAngle);

            if (EditorApplication.isPlaying && GUILayout.Button("Jump!"))
                foreach (SmoothFollow sf in targets)
                    sf.JumpRandomly();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
