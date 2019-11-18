using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations: MonoBehaviour
{
    public AnimationClip oneHandedMelee, oneHandedRanged, oneHandedIdle, oneHandedWalk;
    public AnimationClip twoHandedMelee, twoHandedRanged, twoHandedIdle, twoHandedWalk;

    public AnimationClip DefaultAttackClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedMelee;
            case "Two_handedMelee":
                return twoHandedMelee;
            case "One_handedRanged":
                return oneHandedRanged;
            case "Two_handedRanged":
                return twoHandedRanged;
        }
        return null;
    }

    public AnimationClip DefaultIdleClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedIdle;
            case "Two_handedMelee":
                return twoHandedIdle;
            case "One_handedRanged":
                return oneHandedIdle;
            case "Two_handedRanged":
                return twoHandedIdle;
        }
        return null;
    }

    public AnimationClip DefaultWalkClip(string weapon)
    {
        switch (weapon)
        {
            case "One_handedMelee":
                return oneHandedWalk;
            case "Two_handedMelee":
                return twoHandedWalk;
            case "One_handedRanged":
                return oneHandedWalk;
            case "Two_handedRanged":
                return twoHandedWalk;
        }
        return null;
    }
}
