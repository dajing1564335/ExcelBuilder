namespace Table
{
	[System.Serializable]
	public struct StudentData
	{
		public string name;
		public int age;
		public Score score;
		public Student like;
		public Type type;

		public StudentData(string name, int age, Score score, Student like, Type type)
		{
			this.name = name;
			this.age = age;
			this.score = score;
			this.like = like;
			this.type = type;
		}
	}
}
