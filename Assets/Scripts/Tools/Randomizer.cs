using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, InlineProperty, HideLabel]
public class Randomizer<T>
{
    [SerializeField, ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true)]
    protected List<Option<T>> options = new List<Option<T>>();
    public Option<T> this[T content] => options.Find(OptionMatch(content));
    private bool _isSorted = false;
    public int Count => options.Count;
#if UNITY_EDITOR
    [Button]
    [ResponsiveButtonGroup(DefaultButtonSize = ButtonSizes.Small, UniformLayout = true, Order = 999)]
    private void DebugRandomOption() => Debug.Log(GetRandomOption()?.ToString() ?? "Null");
    [Button]
    [ResponsiveButtonGroup]
    private void DebugTotalWeight() => Debug.Log(GetTotalWeight());
#endif
    public float GetTotalWeight()
    {
        var weight = 0f;
        foreach (var option in options)
            weight += option.Weight;

        return weight;
    }
    public T GetRandomOption()
    {

        if (options.Count == 0)
            throw new ArgumentNullException();

        if (!_isSorted)
            SortOptions();

        var value = UnityEngine.Random.Range(0, GetTotalWeight());

        for (int i = 0; i < options.Count; i++)
        {
            value -= options[i].Weight;
            if (value <= 0)
                return options[i].Content;
        }
        return options[0].Content;
    }
    public T[] GetRandomOptions(int count)
    {
        if (options.Count == 0)
            throw new ArgumentNullException();

        if (count == 0)
            return new T[0];

        if (!_isSorted)
            SortOptions();

        T[] newOptions = new T[count];
        float[] values = new float[count];

        float TotalWeight = GetTotalWeight();

        int optionsFound = 0;

        for (int i = 0; i < values.Length; i++)
            values[i] = UnityEngine.Random.Range(0, TotalWeight);

        for (int i = 0; i < options.Count; i++)
        {
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j] > 0)
                {
                    values[j] -= options[i].Weight;
                    if (values[j] <= 0)
                    {
                        newOptions[j] = options[i].Content;
                        optionsFound++;

                        if (optionsFound == count)
                            return newOptions;
                    }
                }
            }
        }
        return newOptions;
    }
    /// <summary>
    /// Adding an option to the randomizer.
    /// </summary>
    /// <param name="content">The content to add to the randomizer</param>
    /// <param name="weight">The chance weight the option will be rolled, relative to other options. 1 is the default.</param>
    /// <param name="additive">If true, in case of duplication, where the option already exists in the options pool, it will combine the options weights.</param>
    public void Add(bool additive = true, params Option<T>[] newOptions) => Array.ForEach(newOptions, (x) => Add(x, additive));
    public void Add(List<Option<T>> newOptions, bool additive = true) => newOptions.ForEach((x) => Add(x, additive));
    public void Add(Option<T> newOption, bool additive = true)
    {
        if (newOption.Content == null || newOption.Weight <= 0)
            return;

        //Whether I can take into account the randomizer is sorted or not, if it's not I'll just insert the option at the end and sort when first pulling a new option.
        if (!_isSorted)
        {
            var option = this[newOption.Content];
            if (option == null)
                options.Add(newOption);
            else if (additive)
                option.Weight += newOption.Weight;
        }
        else
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (additive && options[i].Content.Equals(newOption.Content))
                {
                    options[i].Weight += newOption.Weight;
                    return;
                }

                if (options[i].Weight <= newOption.Weight)
                {
                    options.Insert(i, newOption);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Removing an option from the randomizer.
    /// </summary>
    /// <param name="content">The content to remove from the randomizer.</param>
    /// <returns></returns>
    public bool Remove(T content)
    {
        int index = options.FindIndex(OptionMatch(content));
        if (index == -1)
            return false;

        options.RemoveAt(index);
        return true;

    }
    /// <summary>
    /// Increase (or decrease) the chance of one of the options.
    /// </summary>
    /// <param name="content">The content of the option to affect.</param>
    /// <param name="weightAmount">The amount to increase (or decrease).</param>
    public void AddWeight(T content, float weightAmount)
    {
        if (content == null || weightAmount == 0)
            return;
        var option = this[content];

        if (option == null)
            return;

        option.Weight += weightAmount;
    }
    private Predicate<IOption<T>> OptionMatch(T option)
        => (x) => x.Content.Equals(option);
    //[Button, PropertyTooltip("Sorrted randomizers increase option pooling performance.")]
    [ResponsiveButtonGroup]
    private void SortOptions()
    {
        options.Sort((a, b) => b.Weight.CompareTo(a.Weight));
        _isSorted = true;
    }


}
/// <summary>
/// The interface for any kind of option for the randomizer.
/// </summary>
/// <typeparam name="T">The type of content the option holds</typeparam>
public interface IOption<T>
{
    /// <summary>
    /// The content the option holds.
    /// </summary>
    T Content { get; set; }
    /// <summary>
    /// The chance weight the option will be rolled, relative to other options. 1 is the default.
    /// </summary>
    float Weight { get; set; }
}
/// <summary>
/// An option the randomizer holds.
/// </summary>
[Serializable]
public class Option<T> : IOption<T>
{
    [SerializeField, HorizontalGroup(LabelWidth = 70)]
    private T content = default;
    [SerializeField, HorizontalGroup(LabelWidth = 70)]
    private float weight = 1;

    public Option(T content, float weight = 1)
    {
        this.content = content;
        this.weight = weight;
    }

    public virtual T Content { get => content; set => content = value; }
    public virtual float Weight { get => weight; set => weight = value; }
}
