using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridReference: ISerializationCallbackReceiver
{
	[SerializeField] private List<List<GridTileView>> gridElements = new List<List<GridTileView>>();

	public List<List<GridTileView>> GridElements
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

    public void MakeEmptyGrid(int columns, int rows)
    {
        if(gridElements != null && gridElements.Count > 0)
        {
            gridElements.Clear();
        }

        gridElements = new List<List<GridTileView>>(columns);
        for (int col = 0; col < columns; col++)
        {
            gridElements.Add(new List<GridTileView>(rows));
            for (int row = 0; row < rows; row++)
            {
                gridElements[col].Add(null);
            }
        }
    }

	#region ISerializationCallbackReceiver implementation
	
	// Serialisation variables
	[SerializeField, HideInInspector] private List<GridTileView> serializeList;
	[SerializeField, HideInInspector] private int serializedColumns;
	[SerializeField, HideInInspector] private int serializedRows;
	
	public void OnBeforeSerialize ()
	{
		if(gridElements != null)
		{
			serializedColumns = gridElements.Count;
			serializedRows = (gridElements.Count > 0)? gridElements[0].Count: 0;
			serializeList = new List<GridTileView>(serializedColumns*serializedRows);
			foreach (List<GridTileView> col in gridElements)
			{
				foreach (GridTileView el in col)
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
		gridElements = new List<List<GridTileView>>(Columns);
		
		if(serializeList.Count > 0)
		{
			int counter = 0;
			for (int col = 0; col < serializedColumns; col++)
			{
				gridElements.Add(new List<GridTileView>(Rows));
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
