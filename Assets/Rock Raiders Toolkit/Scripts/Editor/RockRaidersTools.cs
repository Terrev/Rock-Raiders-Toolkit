﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;

public class RockRaidersTools : MonoBehaviour
{
	[MenuItem("Rock Raiders/Add Planar Texture Helper")]
	static void AddPlanarTextureHelper()
	{
		GameObject obj = (GameObject)Instantiate(Resources.Load("Planar Texture Helper", typeof(GameObject)));
		obj.name = "Planar Texture Helper";
		Undo.RegisterCreatedObjectUndo(obj, "Created Planar Texture Helper");
	}
	
	[MenuItem("Rock Raiders/Add Measuring Tape")]
	static void AddMeasuringTape()
	{
		GameObject obj = (GameObject)Instantiate(Resources.Load("Measuring Tape", typeof(GameObject)));
		obj.name = "Measuring Tape";
		Undo.RegisterCreatedObjectUndo(obj, "Created Measuring Tape");
	}
	
	public struct Polygon
	{
		public UInt16[] indices;
		public Vector2[] uv;
		public int[] uvIndices;
		public Int16 surface;
	}
	
	[MenuItem("Rock Raiders/Save LWO")]
	static void SaveWithoutUV()
	{
		SaveLWO(false);
	}
	
	[MenuItem("Rock Raiders/Save LWO + UV")]
	static void SaveWithUV()
	{
		SaveLWO(true);
	}
	
	static void SaveLWO(bool exportUV)
	{
		// Get stuff from selected model
		if (Selection.activeTransform == null)
		{
			Debug.LogWarning("Please select the model you want to export");
			return;
		}
		
		GameObject modelObject = Selection.activeTransform.gameObject;
		string modelName = modelObject.name;
		
		MeshRenderer meshRenderer = modelObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			Debug.LogWarning("Selected GameObject doesn't have a MeshRenderer component");
			return;
		}
		
		MeshFilter meshFilter = modelObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			Debug.LogWarning("Selected GameObject doesn't have a MeshFilter component");
			return;
		}
		
		Mesh mesh = meshFilter.sharedMesh;
		
		if (exportUV && mesh.uv.Length == 0)
		{
			Debug.LogWarning("Selected mesh doesn't have UVs");
			return;
		}
		
		List<Material> surfaces = new List<Material>();
		List<string> surfaceNames = new List<string>();
		foreach (Material material in meshRenderer.sharedMaterials)
		{
			if (material.shader.name != "Rock Raiders" && material.shader.name != "Rock Raiders Transparent")
			{
				Debug.LogWarning(material.name + " doesn't use a Rock Raiders shader");
				return;
			}
			if (!surfaceNames.Contains(material.name))
			{
				surfaces.Add(material);
				surfaceNames.Add(material.name);
			}
		}
		
		string path = EditorUtility.SaveFilePanel("Save LWO", "", modelName + ".lwo", "lwo");
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter2(fileStream);
		long rememberMe;
		
		binaryWriter.Write("FORMtempLWOB".ToCharArray());
		
		// PNTS
		binaryWriter.Write("PNTS".ToCharArray());
		// Temp length
		binaryWriter.Write("temp".ToCharArray());
		rememberMe = fileStream.Position;
		foreach (Vector3 vertex in mesh.vertices)
		{
			binaryWriter.Write(-vertex.x);
			binaryWriter.Write(vertex.y);
			binaryWriter.Write(-vertex.z);
		}
		FinishWritingChunk(fileStream, binaryWriter, rememberMe);
		
		// SRFS
		binaryWriter.Write("SRFS".ToCharArray());
		// Temp length
		binaryWriter.Write("temp".ToCharArray());
		rememberMe = fileStream.Position;
		foreach (string surfaceName in surfaceNames)
		{
			binaryWriter.Write(surfaceName.ToCharArray());
			binaryWriter.Write('\0');
			// padding
			if (surfaceName.Length % 2 == 0)
			{
				binaryWriter.Write('\0');
			}
		}
		FinishWritingChunk(fileStream, binaryWriter, rememberMe);
		
		// POLS
		binaryWriter.Write("POLS".ToCharArray());
		// Temp length
		binaryWriter.Write("temp".ToCharArray());
		rememberMe = fileStream.Position;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int surfaceIndex = surfaceNames.IndexOf(meshRenderer.sharedMaterials[i].name) + 1;
			int[] tris = mesh.GetTriangles(i);
			for (int j = 0; j < tris.Length; j += 3)
			{
				binaryWriter.Write((UInt16)3); // How many verts
				binaryWriter.Write((UInt16)tris[j]);
				binaryWriter.Write((UInt16)tris[j + 1]);
				binaryWriter.Write((UInt16)tris[j + 2]);
				binaryWriter.Write((UInt16)surfaceIndex);
			}
		}
		FinishWritingChunk(fileStream, binaryWriter, rememberMe);
		
		// SURFs
		foreach (Material surface in surfaces)
		{
			binaryWriter.Write("SURF".ToCharArray());
			// Temp length
			binaryWriter.Write("temp".ToCharArray());
			rememberMe = fileStream.Position;
			binaryWriter.Write(surface.name.ToCharArray());
			binaryWriter.Write('\0');
			// padding
			if (surface.name.Length % 2 == 0)
			{
				binaryWriter.Write('\0');
			}
			
			// COLR
			binaryWriter.Write("COLR".ToCharArray());
			binaryWriter.Write((UInt16)4);
			Color32 color = (Color32)surface.GetColor("_Color");
			binaryWriter.Write(color.r);
			binaryWriter.Write(color.g);
			binaryWriter.Write(color.b);
			binaryWriter.Write((byte)0);
			
			// FLAG
			binaryWriter.Write("FLAG".ToCharArray());
			binaryWriter.Write((UInt16)2);
			int flag = 0;
			if (surface.GetFloat("_Luminosity") != 0.0f)
			{
				flag++;
			}
			flag += 4; // smoothing
			if (surface.GetFloat("_Additive") != 0.0f)
			{
				flag += 512;
			}
			binaryWriter.Write((UInt16)flag);
			
			// DIFF
			if (surface.GetFloat("_Diffuse") != 0.0f)
			{
				binaryWriter.Write("DIFF".ToCharArray());
				binaryWriter.Write((UInt16)2);
				binaryWriter.Write((UInt16)(surface.GetFloat("_Diffuse") * 256.0f));
			}
			
			// TRAN
			if (surface.GetFloat("_Transparency") != 0.0f)
			{
				binaryWriter.Write("TRAN".ToCharArray());
				binaryWriter.Write((UInt16)2);
				binaryWriter.Write((UInt16)(surface.GetFloat("_Transparency") * 256.0f));
			}
			
			// LUMI
			if (surface.GetFloat("_Luminosity") != 0.0f)
			{
				binaryWriter.Write("LUMI".ToCharArray());
				binaryWriter.Write((UInt16)2);
				binaryWriter.Write((UInt16)(surface.GetFloat("_Luminosity") * 256.0f));
			}
			
			if (!exportUV && surface.GetTexture("_MainTex") != null)
			{
				// CTEX
				binaryWriter.Write("CTEX".ToCharArray());
				binaryWriter.Write((UInt16)18);
				binaryWriter.Write("Planar Image Map".ToCharArray());
				binaryWriter.Write('\0');
				binaryWriter.Write('\0');
				
				// TIMG
				binaryWriter.Write("TIMG".ToCharArray());
				// Temp length
				binaryWriter.Write("te".ToCharArray());
				long rememberMe2 = fileStream.Position;
				string texturePath = null;
				string sequence;
				if (surface.GetFloat("_Sequence") != 0.0f)
				{
					sequence = " (sequence)";
				}
				else
				{
					sequence = "";
				}
				if (surface.GetFloat("_SharedTexture") != 0.0f)
				{
					texturePath = @"\\dummy\" + surface.GetTexture("_MainTex").name + ".bmp" + sequence;
				}
				else
				{
					texturePath = @"D:\dummy\" + surface.GetTexture("_MainTex").name + ".bmp" + sequence;
				}
				binaryWriter.Write(texturePath.ToCharArray());
				binaryWriter.Write('\0');
				// padding
				if (texturePath.Length % 2 == 0)
				{
					binaryWriter.Write('\0');
				}
				FinishWritingSurfChunk(fileStream, binaryWriter, rememberMe2);
				
				// TFLG
				binaryWriter.Write("TFLG".ToCharArray());
				binaryWriter.Write((UInt16)2);
				int textureFlag = 0;
				if (surface.GetFloat("_X") != 0.0f)
				{
					textureFlag++;
				}
				else if (surface.GetFloat("_Y") != 0.0f)
				{
					textureFlag += 2;
				}
				else if (surface.GetFloat("_Z") != 0.0f)
				{
					textureFlag += 4;
				}
				else
				{
					// default to z like the game seems to do if none are specified
					textureFlag += 4;
				}
				if (surface.GetFloat("_PixelBlending") != 0.0f)
				{
					textureFlag += 32;
				}
				binaryWriter.Write((UInt16)textureFlag);
				
				// TSIZ
				binaryWriter.Write("TSIZ".ToCharArray());
				binaryWriter.Write((UInt16)12);
				binaryWriter.Write(surface.GetFloat("_TextureSizeX"));
				binaryWriter.Write(surface.GetFloat("_TextureSizeY"));
				binaryWriter.Write(surface.GetFloat("_TextureSizeZ"));
				
				// TCTR
				binaryWriter.Write("TCTR".ToCharArray());
				binaryWriter.Write((UInt16)12);
				binaryWriter.Write(surface.GetFloat("_TextureCenterX"));
				binaryWriter.Write(surface.GetFloat("_TextureCenterY"));
				binaryWriter.Write(surface.GetFloat("_TextureCenterZ"));
			}
			
			FinishWritingChunk(fileStream, binaryWriter, rememberMe);
		}
		
		// Final length
		fileStream.Seek(4, SeekOrigin.Begin);
		binaryWriter.Write((UInt32)(fileStream.Length - 8));
		
		binaryWriter.Close();
		fileStream.Close();
		
		Debug.Log("Saved file " + path);
		
		
		
		// UV TIME
		if (!exportUV)
		{
			return;
		}
		
		StringBuilder uvString = new StringBuilder();
		uvString.Append("2\n");
		uvString.Append(surfaces.Count).Append("\n");
		foreach (string surfaceName in surfaceNames)
		{
			uvString.Append(surfaceName).Append("\n");
		}
		foreach (Material surface in surfaces)
		{
			string texturePath;
			string sequence;
			if (surface.GetFloat("_Sequence") != 0.0f)
			{
				sequence = " (sequence)";
			}
			else
			{
				sequence = "";
			}
			if (surface.GetTexture("_MainTex") == null)
			{
				texturePath = "NULL";
			}
			else if (surface.GetFloat("_SharedTexture") != 0.0f)
			{
				texturePath = @"\\dummy\" + surface.GetTexture("_MainTex").name + ".bmp" + sequence;
			}
			else
			{
				texturePath = @"D:\dummy\" + surface.GetTexture("_MainTex").name + ".bmp" + sequence;
			}
			uvString.Append(texturePath).Append("\n");
		}
		uvString.Append(mesh.triangles.Length / 3).Append("\n");
		int whatever = 0;
		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			uvString.Append(whatever).Append(" 3\n");
			uvString.Append(mesh.uv[mesh.triangles[i]].x).Append(" ").Append(-mesh.uv[mesh.triangles[i]].y + 1.0f).Append(" 0\n");
			uvString.Append(mesh.uv[mesh.triangles[i + 1]].x).Append(" ").Append(-mesh.uv[mesh.triangles[i + 1]].y + 1.0f).Append(" 0\n");
			uvString.Append(mesh.uv[mesh.triangles[i + 2]].x).Append(" ").Append(-mesh.uv[mesh.triangles[i + 2]].y + 1.0f).Append(" 0\n");
			whatever++;
		}
		// dummy data
		for (int i = 0; i < surfaces.Count; i ++)
		{
			uvString.Append("4\n0.000000 0.000000 0.000000\n0.000000 0.000000 0.000000\n0.000000 0.000000 0.000000\n1.000000 1.000000 1.000000\n");
		}
		
		string uvPath = Path.ChangeExtension(path, ".uv");
		File.WriteAllText(uvPath, uvString.ToString());
		Debug.Log("Saved file " + uvPath);
	}
	
	static void FinishWritingChunk(FileStream fileStream, BinaryWriter binaryWriter, long rememberMe)
	{
		fileStream.Seek(rememberMe - 4, SeekOrigin.Begin);
		binaryWriter.Write((UInt32)(fileStream.Length - rememberMe));
		fileStream.Seek(0, SeekOrigin.End);
	}
	
	static void FinishWritingSurfChunk(FileStream fileStream, BinaryWriter binaryWriter, long rememberMe)
	{
		fileStream.Seek(rememberMe - 2, SeekOrigin.Begin);
		binaryWriter.Write((UInt16)(fileStream.Length - rememberMe));
		fileStream.Seek(0, SeekOrigin.End);
	}
}