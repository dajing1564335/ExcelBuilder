using System.Collections.Generic;

namespace Table
{
	[System.Serializable]
	public class SlotParamData
	{
		public List<SlotParamAAA> AAA = new();
		public int Slot10;
		public SlotParamBBB BBB;
		public string SpritePath;
		public List<int> AAA2 = new();
		public List<SlotParamAAAA> AAAA = new();
	}

	[System.Serializable]
	public class SlotParamAAA
	{
		public int a;
		public int b;
		public int c;
	}

	[System.Serializable]
	public class SlotParamBBB
	{
		public int a;
		public int b;
	}

	[System.Serializable]
	public class SlotParamAAAA
	{
		public int a;
		public int b;
	}
}
