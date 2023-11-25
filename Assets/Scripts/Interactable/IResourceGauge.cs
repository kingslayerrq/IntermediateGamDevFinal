using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceGauge 
{
    void gainResource(float amount);
    void useResource(float amount);
    
}
