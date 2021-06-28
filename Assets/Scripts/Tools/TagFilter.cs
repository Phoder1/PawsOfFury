using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    [InlineProperty]
    public class TagFilter
    {
        [SerializeField, ValueDropdown("@UnityEditorInternal.InternalEditorUtility.tags", IsUniqueList = true, FlattenTreeView = true, HideChildProperties = true, DropdownHeight = 180)]
        string[] _tags = default;
        public string[] Tags { get => _tags; }

        /// <summary>
        /// Checks whether the tag exists in the tag filter.
        /// </summary>
        /// <param name="tag">
        /// The tag to check</param>
        /// <param name="onEmptyDefault">
        /// The default value if the tag filter is empty.</param>
        /// <returns></returns>
        public bool Contains(string tag, bool onEmptyDefault = false)
               => Array.Exists(Tags, (x) => tag == x)
             || (onEmptyDefault && Tags.Length == 0);
        public static implicit operator string[](TagFilter tf) => tf.Tags;
    }
