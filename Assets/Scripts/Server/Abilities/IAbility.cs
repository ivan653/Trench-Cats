
using System.Collections;
using Unity.Netcode;

public interface IAbility
{
    AbilityType Type {get;}
    float Cost { get; }
    float ReloadDuration {get;}
    NetworkVariable<double> CooldownEndTime { get;}

    //IEnumerator Reload();
}

public interface IAbility<T> : IAbility
{
    void Use(T inputData);
}