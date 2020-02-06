using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanarTextureHelper))]
public class PlanarTextureHelperEditor : Editor
{
	public override void OnInspectorGUI()
	{
		PlanarTextureHelper planarTextureHelper = (PlanarTextureHelper)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		if (GUILayout.Button("Apply to material"))
		{
			planarTextureHelper.ApplyToMaterial();
		}
	}
}
