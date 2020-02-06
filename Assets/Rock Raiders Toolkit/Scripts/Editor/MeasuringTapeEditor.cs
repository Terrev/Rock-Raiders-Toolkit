using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeasuringTape))]
public class MeasuringTapeEditor : Editor
{
    public override void OnInspectorGUI()
	{
		MeasuringTape measuringTape = (MeasuringTape)target;
		DrawDefaultInspector();
		if (measuringTape.otherEnd != null)
		{
			GUILayout.Label("Distance: " + Vector3.Distance(measuringTape.transform.position, measuringTape.otherEnd.position));
		}
		else
		{
			GUILayout.Label("Please select another Transform to measure distance between");
		}
		if (GUILayout.Button("Align on X"))
		{
			measuringTape.AlignOnX();
		}
		if (GUILayout.Button("Align on Y"))
		{
			measuringTape.AlignOnY();
		}
		if (GUILayout.Button("Align on Z"))
		{
			measuringTape.AlignOnZ();
		}
	}
}
