using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We have to make sure not to set a value for an animator parameter that does not exist
// since Unity doesn't return an error or throws an exception but instead spams us with warnings.
// So first return parameter id, or 0 if parameter doesn't exist.
// Then only set a parameter value if id is not 0.

public static class AnimatorExt
{
    public static int GetParamId(this Animator animator, string name)
    {
        var param = Array.Find<AnimatorControllerParameter>(animator.parameters, (e) => e.name == name);
        return param == null ? 0 : param.nameHash;
    }

    public static bool TrySetBool(this Animator animator, int id, bool value)
    {
        if (id != 0)
        {
            animator.SetBool(id, value);
            return true;
        }
        else
            return false;
    }

    public static bool TrySetFloat(this Animator animator, int id, float value)
    {
        if (id != 0)
        {
            animator.SetFloat(id, value);
            return true;
        }
        else
            return false;
    }
}
