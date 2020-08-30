using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EnemySpawnExcelData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/MasterRecords/EnemySpawnExcelData.xlsx";
	private static readonly string exportPath = "Assets/Resources/MasterRecords/EnemySpawnExcelData.asset";
	private static readonly string[] sheetNames = { "Stage1","Stage2","Stage3","Stage4","Stage5","Stage6","Stage7","Stage8","Stage9","Stage10", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EnemySpawnDataMB data = (EnemySpawnDataMB)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EnemySpawnDataMB));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EnemySpawnDataMB> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					EnemySpawnDataMB.Sheet s = new EnemySpawnDataMB.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EnemySpawnDataMB.Param p = new EnemySpawnDataMB.Param ();
						
					cell = row.GetCell(0); p.time = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.health = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.positionX = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.positionY = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.positionZ = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.directionX = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.directionY = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.directionZ = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.enemySize = (cell == null ? "" : cell.StringCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
