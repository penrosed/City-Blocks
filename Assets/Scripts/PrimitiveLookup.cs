using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveLookup : MonoBehaviour
{
    [System.Serializable]
    public struct NamedMesh
    {
        public string Name;
        public Object Object;
    };

    [SerializeField]
    private List<NamedMesh> primitiveLookup;
    public static Dictionary<string, Object> primitives;

    private void Awake()
    {
        primitives = new Dictionary<string, Object>();
        foreach(NamedMesh p in primitiveLookup)
        {
            primitives.Add(p.Name, p.Object);
        }
    }
}
