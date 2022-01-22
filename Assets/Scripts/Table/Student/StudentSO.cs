using System.Collections.Generic;
using System.Data;
using Table;

public class StudentSO : ScriptableObjectBase
{
	public List<StudentData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<StudentData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull)
			{
				continue;
			}
			Datas.Add(new StudentData(
				TypeConvert.GetValue<string>(table.Rows[i][1].ToString()),
				TypeConvert.GetValue<int>(table.Rows[i][2].ToString()),
				TypeConvert.GetValue<Score>(table.Rows[i][3].ToString()),
				TypeConvert.GetValue<Student>(table.Rows[i][4].ToString()),
				TypeConvert.GetValue<Type>(table.Rows[i][5].ToString())
				));
		}
	}
}
