using UnityEngine;

public interface ICounterable
{
    bool CanBeCountered();
    void StunFor(float duration);
}
