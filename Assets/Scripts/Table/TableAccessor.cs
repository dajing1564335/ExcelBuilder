using Table;

public static class TableAccessor
{
	public static TableAccessorBase<Lesson, LessonData> Lesson;
	public static TableAccessorBase<Score, ScoreData> Score;
	public static TableAccessorBase<Student, StudentData> Student;

	public static void LoadData()
	{
		Lesson = new TableAccessorBase<Lesson, LessonData>();
		Score = new TableAccessorBase<Score, ScoreData>();
		Student = new TableAccessorBase<Student, StudentData>();
	}
}
