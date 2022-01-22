namespace Table
{
	[System.Serializable]
	public struct ScoreData
	{
		public int math;
		public int english;

		public ScoreData(int math, int english)
		{
			this.math = math;
			this.english = english;
		}
	}
}
