using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverrideTest : MonoBehaviour
{
    public bool isOn;

    public AnimatorOverrideController aoc;
    public AnimationClip overrideClip;

    private AnimatorOverrideController aocCopy;

    List<KeyValuePair<AnimationClip, AnimationClip>> _initClips;

    void Start()
    {
        aocCopy = new AnimatorOverrideController(aoc.runtimeAnimatorController) { name = "aocCopy" };

        List<KeyValuePair<AnimationClip, AnimationClip>> clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        aoc.GetOverrides(clips);
        aocCopy.ApplyOverrides(clips);
        _initClips = clips;
        
        GetComponent<Animator>().runtimeAnimatorController = aocCopy;
    }

    void Update()
    {
        if (!isOn)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            OverrideClip("Idle", overrideClip);
        if (Input.GetKeyDown(KeyCode.R))
            RestoreInitClip("Idle");
    }

    public void OverrideClip(string key, AnimationClip clip)
    {
        aocCopy[key] = clip;
    }

    public void RestoreInitClip(string key)
    {
        foreach(KeyValuePair<AnimationClip, AnimationClip> initClip in _initClips)
        {
            if (initClip.Key.name == key)
            {
                Debug.Log(initClip.Value);
                aocCopy[key] = initClip.Value;
            }
        }
    }
}
