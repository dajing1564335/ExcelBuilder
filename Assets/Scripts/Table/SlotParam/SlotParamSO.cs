using System.Collections.Generic;
using System.Data;

public class SlotParamSO : ScriptableObjectBase
{
	public List<Table.SlotParamData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new List<Table.SlotParamData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == "comment")
			{
				continue;
			}
			Table.SlotParamData data = new();
			for (int j = 2; j < 8; j += 3)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
			Table.SlotParamAAA AAA = new();
			AAA.a = TypeConvert.GetValue(table.Rows[i][j].ToString(), "NextType;MapSlot");
			AAA.b = TypeConvert.GetValue<int>(table.Rows[i][j + 1].ToString());
			AAA.c = TypeConvert.GetValue(table.Rows[i][j + 2].ToString(), "NextType");
				data.AAA.Add(AAA);
			}

			data.Slot10 = TypeConvert.GetValue(table.Rows[i][9].ToString(), "MapSlot");
			Table.SlotParamBBB BBB = new();
			BBB.a = TypeConvert.GetValue(table.Rows[i][11].ToString(), "NextType");
			BBB.b = TypeConvert.GetValue<int>(table.Rows[i][12].ToString());
			data.BBB = BBB;
			data.SpritePath = TypeConvert.GetValue<string>(table.Rows[i][14].ToString());
			for (int j = 16; j < 18; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.AAA2.Add(TypeConvert.GetValue<int>(table.Rows[i][j].ToString()));
			}

			for (int j = 19; j < 25; j += 2)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
			Table.SlotParamAAAA AAAA = new();
			AAAA.a = TypeConvert.GetValue(table.Rows[i][j].ToString(), "NextType;MapSlot");
			AAAA.b = TypeConvert.GetValue<int>(table.Rows[i][j + 1].ToString());
				data.AAAA.Add(AAAA);
			}

			Datas.Add(data);
		}
	}
}
