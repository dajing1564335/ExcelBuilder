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
			var data = new StudentData(
				TypeConvert.GetValue<MsgLabel>(table.Rows[i][1].ToString()), 
				TypeConvert.GetValue<int>(table.Rows[i][2].ToString()), 
				TypeConvert.GetValue<Score>(table.Rows[i][3].ToString()), 
				new List<Student>(), 
				TypeConvert.GetValue<Type>(table.Rows[i][8].ToString()), 
				new List<int>(), 
				new List<MsgLabel>()
				);
			for (int j = 5; j < 8; j++)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.like.Add(TypeConvert.GetValue<Student>(table.Rows[i][j].ToString()));
			}
			for (int j = 10; j < 17; j++)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.Rank.Add(TypeConvert.GetValue<int>(table.Rows[i][j].ToString()));
			}
			for (int j = 18; j < 23; j++)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.speak.Add(TypeConvert.GetValue<MsgLabel>(table.Rows[i][j].ToString()));
			}
			Datas.Add(data);
		}
	}
}
