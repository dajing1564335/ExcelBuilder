using System.Collections.Generic;
using System.Data;

public class TextSO : ScriptableObjectBase
{
	public List<Table.TextData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<Table.TextData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == "comment")
			{
				continue;
			}
			Table.TextData data = new();
			for (int j0 = 2; j0 < 20; j0 += 6)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				Table.TextAA AA = new();
				AA.Name = TypeConvert.GetValue<string>(table.Rows[i][j0].ToString());
				AA.na = TypeConvert.GetValue<string>(table.Rows[i][j0 + 1].ToString());
				for (int j1 = j0 + 3; j1 < j0 + 6; j1 += 1)
				{
					if (table.Rows[i][j1] is System.DBNull)
					{
						break;
					}
					AA.Param.Add(TypeConvert.GetValue<int>(table.Rows[i][j1].ToString()));
				}
				data.AA.Add(AA);
			}
			for (int j0 = 22; j0 < 44; j0 += 11)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				Table.TextAAA AAA = new();
				AAA.Name = TypeConvert.GetValue<string>(table.Rows[i][j0].ToString());
				for (int j1 = j0 + 2; j1 < j0 + 10; j1 += 4)
				{
					if (table.Rows[i][j1] is System.DBNull)
					{
						break;
					}
					Table.TextAAAA AAAA = new();
					AAAA.Name = TypeConvert.GetValue<string>(table.Rows[i][j1].ToString());
					for (int j2 = j1 + 2; j2 < j1 + 4; j2 += 1)
					{
						if (table.Rows[i][j2] is System.DBNull)
						{
							break;
						}
						AAAA.Param.Add(TypeConvert.GetValue<int>(table.Rows[i][j2].ToString()));
					}
					AAA.AAAA.Add(AAAA);
				}
				data.AAA.Add(AAA);
			}
			Datas.Add(data);
		}
	}
}
