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
			for (int j0 = 4; j0 < 7; j0 += 1)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				data.PreSlot.Add(TypeConvert.GetValue(table.Rows[i][j0].ToString(), "MapSlot"));
			}
			for (int j0 = 8; j0 < 11; j0 += 1)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				data.NextSlot.Add(TypeConvert.GetValue(table.Rows[i][j0].ToString(), "MapSlot"));
			}
			for (int j0 = 12; j0 < 15; j0 += 1)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				data.NextType.Add(TypeConvert.GetValue(table.Rows[i][j0].ToString(), "NextType"));
			}
			for (int j0 = 16; j0 < 18; j0 += 1)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				data.Test.Add(TypeConvert.GetValue(table.Rows[i][j0].ToString(), "NextType;MapSlot;AA.ABC"));
			}
			data.Point12 = TypeConvert.GetValue(table.Rows[i][18].ToString(), "NextType;SlotParam");
			data.Slot9 = TypeConvert.GetValue(table.Rows[i][19].ToString(), "MapSlot");
			for (int j0 = 21; j0 < 27; j0 += 2)
			{
				if (table.Rows[i][j0] is System.DBNull)
				{
					break;
				}
				Table.MapSlotAAA AAA = new();
				AAA.a = TypeConvert.GetValue(table.Rows[i][j0].ToString(), "NextType");
				AAA.b = TypeConvert.GetValue<int>(table.Rows[i][j0 + 1].ToString());
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
