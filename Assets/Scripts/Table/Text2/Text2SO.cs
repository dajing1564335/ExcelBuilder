using System.Collections.Generic;
using System.Data;

public class Text2SO : ScriptableObjectBase
{
	public List<Table.Text2Data> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<Table.Text2Data>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == "comment")
			{
				continue;
			}
			Table.Text2Data data = new();
			for (int j0 = 2; j0 < 44; j0 += 21)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				Table.Text2AAA AAA = new();
				AAA.String = TypeConvert.GetValue<string>(table.Rows[i][j0].ToString());
				Table.Text2BBB BBB = new();
				BBB.AA = TypeConvert.GetValue<float>(table.Rows[i][j0 + 2].ToString());
				BBB.BB = TypeConvert.GetValue<int>(table.Rows[i][j0 + 3].ToString());
				AAA.BBB = BBB;
				for (int j1 = j0 + 6; j1 < j0 + 10; j1 += 2)
				{
					if (table.Rows[i][j1] is System.DBNull)
					{
						break;
					}
					Table.Text2CCC CCC = new();
					CCC.AA = TypeConvert.GetValue<float>(table.Rows[i][j1].ToString());
					CCC.BB = TypeConvert.GetValue<int>(table.Rows[i][j1 + 1].ToString());
					AAA.CCC.Add(CCC);
				}
				for (int j1 = j0 + 12; j1 < j0 + 20; j1 += 4)
				{
					if (table.Rows[i][j1] is System.DBNull)
					{
						break;
					}
					Table.Text2DDD DDD = new();
					DDD.Name = TypeConvert.GetValue<string>(table.Rows[i][j1].ToString());
					for (int j2 = j1 + 2; j2 < j1 + 4; j2 += 1)
					{
						if (table.Rows[i][j2] is System.DBNull)
						{
							break;
						}
						DDD.INt.Add(TypeConvert.GetValue<int>(table.Rows[i][j2].ToString()));
					}
					AAA.DDD.Add(DDD);
				}
				data.AAA.Add(AAA);
			}
			data.Float1 = TypeConvert.GetValue<float>(table.Rows[i][45].ToString());
			Table.Text2AAAAA AAAAA = new();
			AAAAA.AA = TypeConvert.GetValue<float>(table.Rows[i][47].ToString());
			AAAAA.BB = TypeConvert.GetValue<int>(table.Rows[i][48].ToString());
			data.AAAAA = AAAAA;
			Datas.Add(data);
		}
	}
}
