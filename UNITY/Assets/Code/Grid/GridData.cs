using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridData: ISerializationCallbackReceiver
{
	[SerializeField] private List<List<GridTile>> gridElements = new List<List<GridTile>>();

	public List<List<GridTile>> GridElements
	{
		get 
		{
			return gridElements;
		}
	}

	public int Columns 
	{
		get
		{
			return gridElements.Count;
		}
	}

	public int Rows
	{
		get
		{
			if(gridElements.Count > 0 && gridElements[0] != null)
			{
				return gridElements[0].Count;
			}
			else
			{
				return 0;
			}
		}
	}

	#region ISerializationCallbackReceiver implementation
	
	// Serialisation variables
	[SerializeField][HideInInspector] private List<GridTile> serializeList;
	[SerializeField][HideInInspector] private int serializedColumns;
	[SerializeField][HideInInspector] private int serializedRows;
	
	public void OnBeforeSerialize ()
	{
		if(gridElements != null)
		{
			serializedColumns = gridElements.Count;
			serializedRows = (gridElements.Count > 0)? gridElements[0].Count: 0;
			serializeList = new List<GridTile>(serializedColumns*serializedRows);
			foreach (List<GridTile> col in gridElements)
			{
				foreach (GridTile el in col)
				{
					serializeList.Add(el);
				}
			}
		}
		else
		{
			serializedColumns = 0;
			serializedRows = 0;
		}
	}
	
	public void OnAfterDeserialize ()
	{
		gridElements = new List<List<GridTile>>(Columns);
		
		if(serializeList.Count > 0)
		{
			int counter = 0;
			for (int col = 0; col < serializedColumns; col++)
			{
				gridElements.Add(new List<GridTile>(Rows));
				for (int row = 0; row < serializedRows; row++)
				{
					gridElements[col].Add(serializeList[counter]);
					++counter;
				}
			}
		}
		serializeList.Clear();
		serializedColumns = 0;
		serializedRows = 0;
	}
	
	#endregion
}
