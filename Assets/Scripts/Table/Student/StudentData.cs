using System.Collections.Generic;

namespace Table
{
	[System.Serializable]
	public struct StudentData
	{
		public MsgLabel name;
		public int age;
		public Score score;
		public List<Student> like;
		public Type type;
		public List<int> Rank;
		public List<MsgLabel> speak;

		public StudentData(MsgLabel name, int age, Score score, List<Student> like, Type type, List<int> Rank, List<MsgLabel> speak)
		{
			this.name = name;
			this.age = age;
			this.score = score;
			this.like = like;
			this.type = type;
			this.Rank = Rank;
			this.speak = speak;
		}
	}
}
