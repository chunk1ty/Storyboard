namespace AnkkBoard.Web.Models.DBM
{
    using AnkkBoard.Web.Models.VM;

    public class TaskB
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string Assigner { get; set; }

        public TaskStatus Status { get; set; }
    }
}