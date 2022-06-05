using System.Collections.Generic;

namespace Table
{
	[System.Serializable]
	public class TextData
	{
		public List<TextAA> AA = new();
		public List<TextAAA> AAA = new();
	}

	[System.Serializable]
	public class TextAA
	{
		public string Name;
		public List<int> Param = new();
	}

	[System.Serializable]
	public class TextAAA
	{
		public string Name;
		public List<TextAAAA> AAAA = new();
	}

	[System.Serializable]
	public class TextAAAA
	{
		public string Name;
		public List<int> Param = new();
	}
}
