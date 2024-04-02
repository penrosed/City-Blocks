using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Rendering;
using UnityEditor;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public string blockTitle;
    /*public string blockCreator;*/

    public Block newBlock;

    public void CreateBlock()
    {
        newBlock = new Block();

        List<Prop> palette = new List<Prop>();
        List<PropInstance> layout = new List<PropInstance>();

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var propObj = this.gameObject.transform.GetChild(i);
            var newProp = new Prop { name = propObj.name };

            for (int j = 0; j < propObj.childCount; j++)
            {
                var primObj = propObj.GetChild(j);

                for (int k = 0; k < PrimitiveLookup.primitives.Count; k++)
                {
                    if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(primObj.gameObject) ==
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(PrimitiveLookup.primitives[k]))
                    {
                        List<Primitive> updatedPrimList = new List<Primitive>();

                        if (newProp.primitives != null)
                            updatedPrimList.AddRange(newProp.primitives);

                        var colourAuthoringComponent = primObj.GetComponentInChildren<URPMaterialPropertyBaseColorAuthoring>();
                        updatedPrimList.Add(new Primitive {
                            type = k,
                            transform = primObj,
                            colour = (colourAuthoringComponent) ? colourAuthoringComponent.color : new Color { }
                        });

                        newProp.primitives = updatedPrimList.ToArray();
                    }
                }
            }

            if (!palette.Any(paletteItem => paletteItem == newProp))
            {
                palette.Add(newProp);
            }

            layout.Add(new PropInstance { index = palette.IndexOf(newProp), transform = propObj});
        }

        newBlock.palette = palette.ToArray();
        newBlock.layout = layout.ToArray();
        newBlock.name = this.blockTitle;

        File.WriteAllText(Application.streamingAssetsPath + "/" + newBlock.name + ".json", JsonUtility.ToJson(newBlock));
    }

    public void ApplyMaterialOverrides()
    {
        foreach (var MeshRenderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            var colourOverride = MeshRenderer.gameObject.GetComponent<URPMaterialPropertyBaseColorAuthoring>();

            if (colourOverride != null)
                MeshRenderer.material.color = colourOverride.color;
        }
    }
}