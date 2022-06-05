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
			for (int j0 = 2; j0 < 7; j0 += 1)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				data.Points.Add(TypeConvert.GetValue<int>(table.Rows[i][j0].ToString()));
			}
			Datas.Add(data);
		}
	}
}
