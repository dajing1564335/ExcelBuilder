using System.Collections.Generic;
using System.Data;
using Table;

public class ScoreSO : ScriptableObjectBase
{
	public List<ScoreData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<ScoreData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull)
			{
				continue;
			}
			Datas.Add(new ScoreData(
				TypeConvert.GetValue<int>(table.Rows[i][1].ToString()),
				TypeConvert.GetValue<int>(table.Rows[i][2].ToString())
				));
		}
	}
}
