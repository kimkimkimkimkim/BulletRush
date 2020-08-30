using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnDataMB : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public float time;
		public float health;
		public float positionX;
		public float positionY;
		public float positionZ;
		public float directionX;
		public float directionY;
		public float directionZ;
		public string enemySize;
	}
}

