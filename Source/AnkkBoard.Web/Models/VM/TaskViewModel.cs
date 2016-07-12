namespace AnkkBoard.Web.Models.VM
{
    public class TaskViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string Assigner { get; set; }

        public int Id { get; set; }

        public TaskStatus TaskStatus { get; set; }
    }
}