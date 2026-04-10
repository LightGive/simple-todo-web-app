using simple_todo_web_app.Models.Enums;

namespace simple_todo_web_app.Models.Parameters
{
	public record TaskNameCategorySet
	{
		public TaskCategory Category { get; init; }
		public TaskName TaskName { get; init; }

		public TaskNameCategorySet(TaskCategory category, TaskName taskName)
		{
			if (taskName == null)
			{
				throw new ArgumentException("タスク名は必須です。", nameof(taskName));
			}

			Category = category;
			TaskName = taskName;
		}
	}
}
