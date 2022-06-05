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
			for (int j = 2; j < 17; j += 5)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				Table.TextAA AA = new();
				AA.Name = TypeConvert.GetValue<string>(table.Rows[i][j].ToString());
				for (int j = 5; j < 8; j += 1)
				{
					if (table.Rows[i][j] is System.DBNull)
					{
						break;
					}
					AA.Param.Add(TypeConvert.GetValue<int>(table.Rows[i][j].ToString()));
				}

				data.AA.Add(AA);
			}

			for (int j = 19; j < 41; j += 11)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				Table.TextAAA AAA = new();
				AAA.Name = TypeConvert.GetValue<string>(table.Rows[i][j].ToString());
				for (int j = 22; j < 30; j += 4)
				{
					if (table.Rows[i][j] is System.DBNull)
					{
						break;
					}
					Table.TextAAAA AAAA = new();
					AAAA.Name = TypeConvert.GetValue<string>(table.Rows[i][j].ToString());
					for (int j = 25; j < 27; j += 1)
					{
						if (table.Rows[i][j] is System.DBNull)
						{
							break;
						}
						AAAA.Param.Add(TypeConvert.GetValue<int>(table.Rows[i][j].ToString()));
					}

					AAA.AAAA.Add(AAAA);
				}

				data.AAA.Add(AAA);
			}

			Datas.Add(data);
		}
	}
}
