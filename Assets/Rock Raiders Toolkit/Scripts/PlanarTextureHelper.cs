using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PlanarTextureHelper : MonoBehaviour
{
	public Material material;
	
	void Update()
	{
		// don't rotate
		if (transform.localRotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.identity;
		}
	}
	
	public void ApplyToMaterial()
	{
			if (material == null)
			{
				Debug.LogWarning("Please assign a material");
			}
			if (material.shader.name != "Rock Raiders" && material.shader.name != "Rock Raiders Transparent")
			{
				Debug.LogWarning(material.name + " doesn't use a Rock Raiders shader");
			}
			
			Undo.RecordObject(material, "Set Planar Texture Properties");
			
			material.SetFloat("_TextureCenterX", transform.position.x);
			material.SetFloat("_TextureCenterY", transform.position.y);
			material.SetFloat("_TextureCenterZ", transform.position.z);
			
			material.SetFloat("_TextureSizeX", transform.localScale.x);
			material.SetFloat("_TextureSizeY", transform.localScale.y);
			material.SetFloat("_TextureSizeZ", transform.localScale.z);
			
			Debug.Log("Applied to " + material.name);
	}
}
