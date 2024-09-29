using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    // Extension method for TryGetComponentInParent
    public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
    {
        result = component.GetComponentInParent<T>();
        return result != null;
    }
}