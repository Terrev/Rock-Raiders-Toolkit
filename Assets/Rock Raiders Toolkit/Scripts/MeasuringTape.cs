using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeasuringTape : MonoBehaviour
{
    public Transform otherEnd;
	
	public void AlignOnX()
	{
		Undo.RecordObject(transform, "Align On X");
		transform.position = new Vector3(otherEnd.position.x, transform.position.y, transform.position.z);
	}
	
	public void AlignOnY()
	{
		Undo.RecordObject(transform, "Align On Y");
		transform.position = new Vector3(transform.position.x, otherEnd.position.y, transform.position.z);
	}
	
	public void AlignOnZ()
	{
		Undo.RecordObject(transform, "Align On Z");
		transform.position = new Vector3(transform.position.x, transform.position.y, otherEnd.position.z);
	}
	
	// Technically this stuff gets drawn twice, once by each end, but whatever, no harm done
	void OnDrawGizmos()
	{
		if (otherEnd != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position + (Vector3.up * 0.01f), otherEnd.position + (Vector3.up * 0.01f));
			Vector3 labelPosition = (transform.position + otherEnd.position) / 2;
			labelPosition += Vector3.up;
			Handles.Label(labelPosition, Vector3.Distance(transform.position, otherEnd.position).ToString());
		}
	}
}
