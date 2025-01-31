using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeEvent
{
    public static event Action<string, int> OnChallengeProgressUpdated;

    public static void UpdateChallengeProgress(string challengeName, int amount)
    {
        OnChallengeProgressUpdated?.Invoke(challengeName, amount);
    }
}
