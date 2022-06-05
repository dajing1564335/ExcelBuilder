public static class TableAccessor
{
	public static TableAccessorDictionary<Table.MapSlotEnum, Table.MapSlotData> MapSlot;
	public static TableAccessorList<Table.NextTypeEnum, Table.NextTypeData> NextType;
	public static TableAccessorList<Table.SlotParamEnum, Table.SlotParamData> SlotParam;
	public static TableAccessorList<Table.TextEnum, Table.TextData> Text;

	public static void LoadData()
	{
		MapSlot = new TableAccessorDictionary<Table.MapSlotEnum, Table.MapSlotData>();
		NextType = new TableAccessorList<Table.NextTypeEnum, Table.NextTypeData>();
		SlotParam = new TableAccessorList<Table.SlotParamEnum, Table.SlotParamData>();
		Text = new TableAccessorList<Table.TextEnum, Table.TextData>();
	}
}
