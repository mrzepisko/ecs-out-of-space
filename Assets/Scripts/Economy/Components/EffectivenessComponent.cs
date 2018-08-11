using UnityEngine;
using System.Collections;
using Data;

public class EffectivenessComponent : MonoBehaviour {
    public EffectivenessData Value;

    public float Effectiveness { get { return Mathf.Min(Value.ManualEffectiveness, Value.AvailableEffectiveness); } }
}
