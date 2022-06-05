using System.Collections.Generic;

namespace Table
{
	[System.Serializable]
	public class Text2Data
	{
		public List<Text2AAA> AAA = new();
		public float Float1;
		public Text2AAAAA AAAAA;
	}

	[System.Serializable]
	public class Text2AAA
	{
		public string String;
		public Text2BBB BBB;
		public List<Text2CCC> CCC = new();
		public List<Text2DDD> DDD = new();
	}

	[System.Serializable]
	public class Text2BBB
	{
		public float AA;
		public int BB;
	}

	[System.Serializable]
	public class Text2CCC
	{
		public float AA;
		public int BB;
	}

	[System.Serializable]
	public class Text2DDD
	{
		public string Name;
		public List<int> INt = new();
	}

	[System.Serializable]
	public class Text2AAAAA
	{
		public float AA;
		public int BB;
	}
}
