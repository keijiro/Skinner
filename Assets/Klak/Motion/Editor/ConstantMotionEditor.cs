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
    [CustomEditor(typeof(ConstantMotion))]
    public class ConstantMotionEditor : Editor
    {
        SerializedProperty _translationMode;
        SerializedProperty _translationVector;
        SerializedProperty _translationSpeed;

        SerializedProperty _rotationMode;
        SerializedProperty _rotationAxis;
        SerializedProperty _rotationSpeed;

        SerializedProperty _useLocalCoordinate;

        static GUIContent _textLocalCoordinate = new GUIContent("Local Coordinate");
        static GUIContent _textRotation = new GUIContent("Rotation");
        static GUIContent _textSpeed = new GUIContent("Speed");
        static GUIContent _textTranslation = new GUIContent("Translation");
        static GUIContent _textVector = new GUIContent("Vector");

        void OnEnable()
        {
            _translationMode = serializedObject.FindProperty("_translationMode");
            _translationVector = serializedObject.FindProperty("_translationVector");
            _translationSpeed = serializedObject.FindProperty("_translationSpeed");

            _rotationMode = serializedObject.FindProperty("_rotationMode");
            _rotationAxis = serializedObject.FindProperty("_rotationAxis");
            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");

            _useLocalCoordinate = serializedObject.FindProperty("_useLocalCoordinate");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_translationMode, _textTranslation);

            EditorGUI.indentLevel++;

            if (_translationMode.hasMultipleDifferentValues ||
                _translationMode.enumValueIndex == (int)ConstantMotion.TranslationMode.Vector)
                EditorGUILayout.PropertyField(_translationVector, _textVector);

            if (_translationMode.hasMultipleDifferentValues ||
                _translationMode.enumValueIndex != 0)
                EditorGUILayout.PropertyField(_translationSpeed, _textSpeed);

            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_rotationMode, _textRotation);

            EditorGUI.indentLevel++;

            if (_rotationMode.hasMultipleDifferentValues ||
                _rotationMode.enumValueIndex == (int)ConstantMotion.RotationMode.Vector)
                EditorGUILayout.PropertyField(_rotationAxis, _textVector);

            if (_rotationMode.hasMultipleDifferentValues ||
                _rotationMode.enumValueIndex != 0)
                EditorGUILayout.PropertyField(_rotationSpeed, _textSpeed);

            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_useLocalCoordinate, _textLocalCoordinate);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
