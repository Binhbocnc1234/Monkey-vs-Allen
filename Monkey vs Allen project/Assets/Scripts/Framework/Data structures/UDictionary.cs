using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Common
{
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
    
    [Serializable]
    public class UDictionary
    {
        public class SplitAttribute : PropertyAttribute
        {
            public float Key { get; protected set; }
            public float Value { get; protected set; }

            public SplitAttribute(float key, float value)
            {
                Key = key;
                Value = value;
            }
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(SplitAttribute), true)]
        [CustomPropertyDrawer(typeof(UDictionary), true)]
        public class Drawer : PropertyDrawer
        {
            private SerializedProperty property;

            private bool IsExpanded
            {
                get => property.isExpanded;
                set => property.isExpanded = value;
            }

            private SerializedProperty keys;
            private SerializedProperty values;

            private bool IsAligned => keys.arraySize == values.arraySize;

            private ReorderableList list;

            private GUIContent label;

            private SplitAttribute split;

            private float KeySplit => split?.Key ?? 30f;
            private float ValueSplit => split?.Value ?? 70f;

            private static float SingleLineHeight => EditorGUIUtility.singleLineHeight;

            private const float ElementHeightPadding = 6f;
            private const float ElementSpacing = 10f;
            public const float ElementFoldoutPadding = 20f;

            private const float TopPadding = 5f;
            private const float BottomPadding = 5f;

            private void Init(SerializedProperty value)
            {
                if (SerializedProperty.EqualContents(value, property)) return;

                property = value;

                keys = property.FindPropertyRelative(nameof(keys));
                values = property.FindPropertyRelative(nameof(values));

                split = attribute as SplitAttribute;

                list = new ReorderableList(property.serializedObject, keys, true, true, true, true)
                    {
                        drawHeaderCallback = DrawHeader,
                        onAddCallback = Add,
                        onRemoveCallback = Remove,
                        elementHeightCallback = GetElementHeight,
                        drawElementCallback = DrawElement
                    };

                list.onReorderCallbackWithDetails += Reorder;
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                Init(property);

                var height = TopPadding + BottomPadding;

                if (IsAligned)
                    height += IsExpanded ? list.GetHeight() : list.headerHeight;
                else
                    height += SingleLineHeight;

                return height;
            }

            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                label.text = $" {label.text}";

                this.label = label;

                Init(property);

                rect = EditorGUI.IndentedRect(rect);

                rect.y += TopPadding;
                rect.height -= TopPadding + BottomPadding;

                if (IsAligned == false)
                {
                    DrawAlignmentWarning(ref rect);
                    return;
                }

                if (IsExpanded)
                    DrawList(ref rect);
                else
                    DrawCompleteHeader(ref rect);
            }

            private void DrawList(ref Rect rect)
            {
                EditorGUIUtility.labelWidth = 80f;
                EditorGUIUtility.fieldWidth = 80f;

                list.DoList(rect);
            }

            private void DrawAlignmentWarning(ref Rect rect)
            {
                const float width = 80f;
                const float spacing = 5f;

                rect.width -= width;

                EditorGUI.HelpBox(rect, "  Misalignment Detected", MessageType.Error);

                rect.x += rect.width + spacing;
                rect.width = width - spacing;

                if (GUI.Button(rect, "Fix"))
                {
                    if (keys.arraySize > values.arraySize)
                    {
                        var difference = keys.arraySize - values.arraySize;

                        for (var i = 0; i < difference; i++)
                            keys.DeleteArrayElementAtIndex(keys.arraySize - 1);
                    }
                    else if (keys.arraySize < values.arraySize)
                    {
                        var difference = values.arraySize - keys.arraySize;

                        for (var i = 0; i < difference; i++)
                            values.DeleteArrayElementAtIndex(values.arraySize - 1);
                    }
                }
            }

            #region Draw Header

            private void DrawHeader(Rect rect)
            {
                rect.x += 10f;

                IsExpanded = EditorGUI.Foldout(rect, IsExpanded, label, true);
            }

            private void DrawCompleteHeader(ref Rect rect)
            {
                ReorderableList.defaultBehaviours.DrawHeaderBackground(rect);

                rect.x += 6;
                rect.y += 0;

                DrawHeader(rect);
            }
            #endregion

            private float GetElementHeight(int index)
            {
                var key = keys.GetArrayElementAtIndex(index);
                var value = values.GetArrayElementAtIndex(index);

                var kHeight = GetChildrenSingleHeight(key);
                var vHeight = GetChildrenSingleHeight(value);

                var max = Math.Max(kHeight, vHeight);

                if (max < SingleLineHeight) max = SingleLineHeight;

                return max + ElementHeightPadding;
            }

            #region Draw Element

            private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                rect.height -= ElementHeightPadding;
                rect.y += ElementHeightPadding / 2;

                var areas = Split(rect, KeySplit, ValueSplit);

                DrawKey(areas[0], index);
                DrawValue(areas[1], index);
            }

            private void DrawKey(Rect rect, int index)
            {
                var property = keys.GetArrayElementAtIndex(index);

                rect.x += ElementSpacing / 2f;
                rect.width -= ElementSpacing;

                DrawField(rect, property);
            }

            private void DrawValue(Rect rect, int index)
            {
                var property = values.GetArrayElementAtIndex(index);

                rect.x += ElementSpacing / 2f;
                rect.width -= ElementSpacing;

                DrawField(rect, property);
            }

            private static void DrawField(Rect rect, SerializedProperty property)
            {
                rect.height = SingleLineHeight;

                if (IsInline(property))
                {
                    EditorGUI.PropertyField(rect, property, GUIContent.none);
                }
                else
                {
                    rect.x += ElementSpacing / 2f;
                    rect.width -= ElementSpacing;

                    foreach (var child in IterateChildren(property))
                    {
                        EditorGUI.PropertyField(rect, child, false);

                        rect.y += SingleLineHeight + +2f;
                    }
                }
            }
            #endregion

            private void Reorder(ReorderableList list, int oldIndex, int newIndex)
            {
                values.MoveArrayElement(oldIndex, newIndex);
            }

            private void Add(ReorderableList list)
            {
                values.InsertArrayElementAtIndex(values.arraySize);

                ReorderableList.defaultBehaviours.DoAddButton(list);
            }

            private void Remove(ReorderableList list)
            {
                values.DeleteArrayElementAtIndex(list.index);

                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }

            //Static Utility
            private static Rect[] Split(Rect source, params float[] cuts)
            {
                var rects = new Rect[cuts.Length];

                var x = 0f;

                for (var i = 0; i < cuts.Length; i++)
                {
                    rects[i] = new Rect(source);

                    rects[i].x += x;
                    rects[i].width *= cuts[i] / 100;

                    x += rects[i].width;
                }

                return rects;
            }

            private static bool IsInline(SerializedProperty property)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Generic:
                        return property.hasVisibleChildren == false;
                }

                return true;
            }

            private static IEnumerable<SerializedProperty> IterateChildren(SerializedProperty property)
            {
                var path = property.propertyPath;

                property.Next(true);

                while (true)
                {
                    yield return property;

                    if (property.NextVisible(false) == false) break;
                    if (property.propertyPath.StartsWith(path) == false) break;
                }
            }

            private static float GetChildrenSingleHeight(SerializedProperty property)
            {
                return IsInline(property) ? SingleLineHeight : IterateChildren(property).Sum(_ => SingleLineHeight + 2f);
            }
        }
#endif
    }

    [Serializable]
    public class UDictionary<TKey, TValue> : UDictionary, IDictionary<TKey, TValue>
    {
        [SerializeField] private List<TKey> keys = new();
        public List<TKey> Keys => keys;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => keys;

        [SerializeField] private List<TValue> values = new();
        public List<TValue> Values => values;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => values;

        public int Count => keys.Count;

        public bool IsReadOnly => false;

        private Dictionary<TKey, TValue> cache;

        public bool Cached => cache != null;

        public Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                if (cache == null)
                {
                    cache = new Dictionary<TKey, TValue>();

                    for (var i = 0; i < keys.Count; i++)
                    {
                        if (keys[i] == null) continue;
                        if (cache.ContainsKey(keys[i])) continue;

                        cache.Add(keys[i], values[i]);
                    }
                }

                return cache;
            }
        }

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set
            {
                var index = keys.IndexOf(key);

                if (index < 0)
                {
                    Add(key, value);
                }
                else
                {
                    values[index] = value;
                    if (Cached) Dictionary[key] = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

        public void Add(TKey key, TValue value)
        {
            keys.Add(key);
            values.Add(value);

            if (Cached) Dictionary.Add(key, value);
        }
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public bool Remove(TKey key)
        {
            var index = keys.IndexOf(key);

            if (index < 0) return false;

            keys.RemoveAt(index);
            values.RemoveAt(index);

            if (Cached) Dictionary.Remove(key);

            return true;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public void Clear()
        {
            keys.Clear();
            values.Clear();

            if (Cached) Dictionary.Clear();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => (Dictionary as IDictionary).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
    }
}