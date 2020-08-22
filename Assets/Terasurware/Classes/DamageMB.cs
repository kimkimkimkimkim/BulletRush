using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageMB : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int Level;
		public float Value;
		public int NextLevelCost;
	}
}