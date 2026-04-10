namespace simple_todo_web_app.Models.Parameters
{
	public record TaskName
	{
		public string TaskNameValue { get; init; }
		public TaskName(string taskName)
		{
			if (string.IsNullOrWhiteSpace(taskName))
			{
				throw new ArgumentException("タスク名は必須です。", nameof(taskName));
			}
			if (taskName.Length > Common.Constants.TaskConstants.NameMaxLength)
			{
				throw new ArgumentException($"タスク名は{Common.Constants.TaskConstants.NameMaxLength}文字以内で入力してください。", nameof(taskName));
			}
			TaskNameValue = taskName;
		}
	}
}
