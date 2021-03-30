using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorLogType { HighlightOnly, Warning, Error}
public class RequiredFieldAttribute : PropertyAttribute
{
    public readonly ErrorLogType errorLogType;

    public RequiredFieldAttribute(ErrorLogType errorLogType)
    {
        this.errorLogType = errorLogType;
    }
}
