using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageMB : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int Id;
		public int EnemySpawnDataSheetIndex;
		public int RewardGem;
		public int RewardCoin;
	}
}