using System.Collections.Generic;
using System.Data;

public class MapSlotSO : ScriptableObjectBase
{
	public SerializableDictionary<Table.MapSlotEnum, Table.MapSlotData> Datas;

	public override void CreateData(DataTable table)
	{
		Datas = new SerializableDictionary<Table.MapSlotEnum, Table.MapSlotData>();
		for (int i = 2; i < table.Rows.Count; i++)
		{
			if (table.Rows[i][0] is System.DBNull || table.Rows[i][0].ToString() == "comment")
			{
				continue;
			}
			Table.MapSlotData data = new();
			data.X = TypeConvert.GetValue<float>(table.Rows[i][1].ToString());
			data.Z = TypeConvert.GetValue<float>(table.Rows[i][2].ToString());
			for (int j = 4; j < 7; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.PreSlot.Add(TypeConvert.GetValue(table.Rows[i][j].ToString(), "MapSlot"));
			}

			for (int j = 8; j < 11; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.NextSlot.Add(TypeConvert.GetValue(table.Rows[i][j].ToString(), "MapSlot"));
			}

			for (int j = 12; j < 15; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.NextType.Add(TypeConvert.GetValue(table.Rows[i][j].ToString(), "NextType"));
			}

			for (int j = 16; j < 18; j += 1)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
				data.Test.Add(TypeConvert.GetValue(table.Rows[i][j].ToString(), "NextType;MapSlot;AA.ABC"));
			}

			data.Point12 = TypeConvert.GetValue(table.Rows[i][18].ToString(), "NextType;SlotParam");
			data.Slot9 = TypeConvert.GetValue(table.Rows[i][19].ToString(), "MapSlot");
			for (int j = 21; j < 27; j += 2)
			{
				if (table.Rows[i][j] is System.DBNull)
				{
					break;
				}
			Table.MapSlotAAA AAA = new();
			AAA.a = TypeConvert.GetValue(table.Rows[i][j].ToString(), "NextType");
			AAA.b = TypeConvert.GetValue<int>(table.Rows[i][j + 1].ToString());
				data.AAA.Add(AAA);
			}

			data.Slot10 = TypeConvert.GetValue(table.Rows[i][28].ToString(), "MapSlot");
			Table.MapSlotBBB BBB = new();
			BBB.a = TypeConvert.GetValue(table.Rows[i][30].ToString(), "NextType");
			BBB.b = TypeConvert.GetValue<int>(table.Rows[i][31].ToString());
			data.BBB = BBB;
			Datas.Add((Table.MapSlotEnum)System.Enum.Parse(typeof(Table.MapSlotEnum), table.Rows[i][0].ToString()), data);
		}
	}
}
