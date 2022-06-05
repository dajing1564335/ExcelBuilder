using System.Collections.Generic;
using System.Data;

public class NextTypeSO : ScriptableObjectBase
{
	public List<Table.NextTypeData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<Table.NextTypeData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == "comment")
			{
				continue;
			}
			Table.NextTypeData data = new();
			for (int j = 2; j < 7; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.Points.Add(TypeConvert.GetValue<int>(table.Rows[i][j].ToString()));
			}

			Datas.Add(data);
		}
	}
}
