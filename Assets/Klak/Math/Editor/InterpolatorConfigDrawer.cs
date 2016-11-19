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

namespace Klak.Math
{
    using Config = FloatInterpolator.Config;

    [CustomPropertyDrawer(typeof(Config))]
    class FloatInterpolatorConfigDrawer : PropertyDrawer
    {
        static GUIContent _textSpeed = new GUIContent("Speed");

        bool CheckShouldExpand(SerializedProperty property)
        {
            var type = property.FindPropertyRelative("_interpolationType");
            return type.hasMultipleDifferentValues ||
                type.enumValueIndex != (int)Config.InterpolationType.Direct;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var lineSpace = EditorGUIUtility.standardVerticalSpacing;
            return CheckShouldExpand(property) ?
                lineHeight * 2 + lineSpace : lineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            const int indentWidth = 16;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var lineSpace = EditorGUIUtility.standardVerticalSpacing;

            // these controls have single line height
            position.height = lineHeight;

            // interpolation type
            var type = property.FindPropertyRelative("_interpolationType");
            EditorGUI.PropertyField(position, type, label);

            if (CheckShouldExpand(property))
            {
                // indent the line
                position.width -= indentWidth;
                position.x += indentWidth;
                EditorGUIUtility.labelWidth -= indentWidth;

                // go to the next line
                position.y += lineHeight + lineSpace;

                // interpolation speed
                var speed = property.FindPropertyRelative("_interpolationSpeed");
                EditorGUI.PropertyField(position, speed, _textSpeed);
            }

            EditorGUI.EndProperty();
        }
    }
}
