# PROJE ADI: 
ToonBlastPrototype 
## TANIM: 
Proje ToonBlast Oyununun GamePlay Prototipidir.
## KOD ACIKLAMASI
-Oyunun base sistemi **Constructor Dependency Injection** tasarım modeli kullanılarak yazılmıştır. <br/>

-**GridSpawnController,GridController,GridMoveController ve GridGoalController** clasları **LevelController'in** Ctor'unda oluşturulmuş, **LevelController** ise **GameManagerin** Startında Oluşturulmuştur.

-Sahne referansları ve ScriptableObject olarak class settings'leri **GameManager** de tutulmuş ve ilgili classlara buradan dağıtılmıştır.<br/>

-BaseEntitiyTypeDefinition adlı Base ScriptableObject Oluşturulmuş.Buradan BlockTypeDefinition ve RocketTypeDefinition gibi classlara kalıtılmıştır. Objectler spawn olurken,Goal ve Level Tanımlamalarında Type özellikleri buradan cekilmiş hem modülerlik hem level design da kolaylık sağlanmıstır.

```
[CreateAssetMenu(menuName = "ScriptableObjects/Base Entity Type Definition")]
public class BaseEntitiyTypeDefinition : ScriptableObject, IGridEntityTypeDefinition
{
    [BHeader("Base Grid Entity Settings")]
    [SerializeField] protected GameObject gridEntityPrefab;
    [SerializeField] protected string gridEntityTypeName;
    [SerializeField] protected Sprite defaultSprite;
    [SerializeField] protected GameObject onDestroyParticle;
    [SerializeField] protected AudioClip onDestroyAudio;
    [SerializeField] protected bool entityIncludedInShuffle = true;
    [SerializeField] protected List<EntityDestroyTypes> immuneToDestroyTypes = new List<EntityDestroyTypes>();

    public string GridEntityTypeName => gridEntityTypeName;

    public Sprite DefaultEntitySprite => defaultSprite;

    public GameObject OnDestroyParticle => onDestroyParticle;

    public AudioClip OnDestroyAudio => onDestroyAudio;

    public GameObject GridEntityPrefab => gridEntityPrefab;

    public bool EntityIncludedInShuffle => entityIncludedInShuffle;
    public List<EntityDestroyTypes> ImmuneToDestroyTypes => immuneToDestroyTypes;
   
}
```
```
using UnityEditor;

[CustomEditor(typeof(LevelSettings))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelSettings levelConfig = (LevelSettings)target;
        GridStartLayout gridStartLayout = levelConfig.GridEntitySpawnerSettings.gridStartLayout;
        if (gridStartLayout.RowCount != levelConfig.GridControllerSettings.RowCount || gridStartLayout.CollumnCount != levelConfig.GridControllerSettings.ColumnCount)
        {
            levelConfig.GridEntitySpawnerSettings.gridStartLayout = new GridStartLayout((int)levelConfig.GridControllerSettings.RowCount, (int)levelConfig.GridControllerSettings.ColumnCount);
        }

    }
}
```
```
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(GridStartLayout))]
public class ArrayPropertyDrawer : PropertyDrawer
{

	private readonly float cellSize = 40;
	private readonly float soHandlePixelSize = 20;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		SerializedProperty data = property.FindPropertyRelative("rows");
		newposition.y += cellSize;

		for (int j = 0; j < data.arraySize; j++)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			newposition.height = cellSize;
			newposition.width = cellSize * 1.6f;

			for (int i = 0; i < row.arraySize; i++)
			{
				EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
				BaseEntitiyTypeDefinition typeDef = (row.GetArrayElementAtIndex(i).objectReferenceValue as BaseEntitiyTypeDefinition);
				newposition.width -= soHandlePixelSize;
				if (typeDef != null) EditorGUI.DrawTextureTransparent(newposition, typeDef.DefaultEntitySprite.texture);
				newposition.width += soHandlePixelSize;
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += cellSize;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return cellSize * (property.FindPropertyRelative("rows").arraySize + 2);
	}
}
```
```
using UnityEngine;
using System.Collections;

[System.Serializable]
public class GridStartLayout
{

    [System.Serializable]
    public struct rowData
    {
        public BaseEntitiyTypeDefinition[] row;
    }

    public rowData[] rows;

    public int RowCount => rows.Length;

    public int CollumnCount => rows[0].row != null ? rows[0].row.Length : 0;

    public GridStartLayout(int rowCount, int collumnCount)
    {
        rowCount = Mathf.Max(1, rowCount);
        collumnCount = Mathf.Max(1, collumnCount);
        rows = new rowData[rowCount];
        for (int i = 0; i < rowCount; i++)
        {
            rows[i].row = new BaseEntitiyTypeDefinition[collumnCount];
        }
    }

    public static GridStartLayout FromArray(int rowCount, int collumnCount, BaseEntitiyTypeDefinition[] entityTypes)
    {
        GridStartLayout layout = new GridStartLayout(rowCount, collumnCount);
        for (int i = collumnCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < rowCount; j++)
            {
                layout.rows[i].row[j] = entityTypes[(collumnCount - 1 - i) * rowCount + j];
            }
        }
        return layout;
    }
}
```

## GAME VIDEO:

![LevelSetting](https://github.com/cengizsezer/ToonBlastPrototype/assets/79985357/c84cf532-439d-4181-b363-f11ab93b997d)<br/>



https://github.com/cengizsezer/ToonBlastPrototype/assets/79985357/bf57ca37-6a49-4612-b609-740aae07875a





