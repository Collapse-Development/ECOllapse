using UnityEngine;

public abstract class SystemConfig<TSystem> : SystemConfig 
    where TSystem : class
{
    public sealed override Type SystemType => typeof(TSystem);
}