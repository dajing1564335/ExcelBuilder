using System.Collections.Generic;
using System.Data;
using Table;

public class LessonSO : ScriptableObjectBase
{
	public List<LessonData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<LessonData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull)
			{
				continue;
			}
			Datas.Add(new LessonData(
				TypeConvert.GetValue<int>(table.Rows[i][1].ToString()),
				TypeConvert.GetValue<float>(table.Rows[i][2].ToString())
				));
		}
	}
}
