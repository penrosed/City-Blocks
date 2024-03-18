using System.Collections.Generic;
using UnityEngine;

public class PrimitiveLookup : MonoBehaviour
{
    [System.Serializable]
    public struct PrimPrefab
    {
        public Object prefab;
    };

    [SerializeField]
    private List<PrimPrefab> primitiveLookup;
    public static List<Object> primitives;

    private void Awake()
    {
        primitives = new List<Object>();
        foreach (PrimPrefab p in primitiveLookup)
        {
            primitives.Add(p.prefab);
        }
    }
}
