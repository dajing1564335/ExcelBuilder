using System.Collections.Generic;

namespace Table
{
	[System.Serializable]
	public class MapSlotData
	{
		public float X;
		public float Z;
		public List<int> PreSlot = new();
		public List<int> NextSlot = new();
		public List<int> NextType = new();
		public List<int> Test = new();
		public int Point12;
		public int Slot9;
		public List<MapSlotAAA> AAA = new();
		public int Slot10;
		public MapSlotBBB BBB;
	}

	[System.Serializable]
	public class MapSlotAAA
	{
		public int a;
		public int b;
	}

	[System.Serializable]
	public class MapSlotBBB
	{
		public int a;
		public int b;
	}
}
