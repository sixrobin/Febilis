using UnityEngine;

public class Recoil
{
    public Recoil(float dir, float force, float dur)
    {
        Dir = dir;
        Force = force;
        Dur = dur;
    }

    public float Dir { get; private set; }
    public float Force { get; private set; }
    public float Dur { get; private set; }

    public bool IsComplete => Dur <= 0f;

    public void Update()
    {
        Dur -= Time.deltaTime;
    }
}