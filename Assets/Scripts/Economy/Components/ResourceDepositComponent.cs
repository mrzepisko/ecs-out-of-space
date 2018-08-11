using UnityEngine;
using System.Collections;
using Data;

public class ResourceDepositComponent : MonoBehaviour {
    [SerializeField] ResourceDepositData[] value;

    public ResourceDepositData[] Value { get { return value; } }
}
