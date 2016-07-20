namespace AnkkBoard.Web.Models.VM
{
    using System.ComponentModel.DataAnnotations;

    public class TaskViewModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(10)]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required]
        public string Creator { get; set; }

        [Required]
        public string Assigner { get; set; }

        public int Id { get; set; }

        public TaskStatus TaskStatus { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }
    }
}