namespace AnkkBoard.Web.Controllers
{
    using AnkkBoard.Web.Models.VM;
    using Models;
    using Models.DBM;
    using System.Web.Mvc;
    using System.Linq;
    using EntityFramework.Extensions;
    using System;
    using System.Collections.Generic;


    public class HomeController : Controller
    {
        private ApplicationDbContext dbContext;
        private Dictionary<int, string> teamMembers = new Dictionary<int, string>()
        {
            { 1, "Andriyan Krastev" },
            { 2, "Milen Kozhuharov" },
            { 3, "Svetoslav Georgiev" },
            { 4, "Teodor Dochev" }
        };

        public HomeController()
        {
            dbContext = new ApplicationDbContext();
        }

        [Authorize(Roles = "TeamVokil")]
        public ActionResult Index()
        {
            var allTasks = dbContext
                .TasksB
                .Select(
                    x => new TaskViewModel
                    {
                        Assigner = x.Assigner,
                        Creator = x.Creator,
                        Description = x.Description,
                        Title = x.Title,
                        Id = x.Id,
                        TaskStatus = x.Status,
                        Priority = x.Priority
                    })
                  .Where(x => x.TaskStatus != TaskStatus.Done)
                  .OrderBy(x => x.Assigner)
                  .ThenBy(x => x.Priority)
                  .ToList();

            return this.View(allTasks);
        }

        [Authorize(Roles = "TeamVokil")]
        public ActionResult DoneTasks()
        {
            var allTasks = dbContext
                .TasksB
                .Select(
                    x => new TaskViewModel
                    {
                        Assigner = x.Assigner,
                        Creator = x.Creator,
                        Description = x.Description,
                        Title = x.Title,
                        Id = x.Id,
                        TaskStatus = x.Status,
                        Priority = x.Priority
                    })
                  .Where(x => x.TaskStatus == TaskStatus.Done)
                  .OrderBy(x => x.Assigner)
                  .ThenBy(x => x.Priority)
                  .ToList();

            return this.View(allTasks);
        }

        [HttpGet]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult NewTask()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult NewTask(TaskViewModel taskVM)
        {
            if (taskVM.Assigner == "0" || taskVM.Creator == "0" || !ModelState.IsValid)
            {
                return this.View(taskVM);
            }

            TaskB taskDb = new TaskB();
            taskDb.Title = taskVM.Title;
            taskDb.Description = taskVM.Description;
            taskDb.Creator = taskVM.Creator;
            taskDb.Assigner = taskVM.Assigner;
            taskDb.Status = TaskStatus.Task;
            taskDb.Priority = taskVM.Priority;

            dbContext.TasksB.Add(taskDb);
            dbContext.SaveChanges();

            ViewBag.TeamMembers = teamMembers;

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Action", Value = "0" });

            items.Add(new SelectListItem { Text = "Drama", Value = "1" });

            items.Add(new SelectListItem { Text = "Comedy", Value = "2", Selected = true });

            items.Add(new SelectListItem { Text = "Science Fiction", Value = "3" });

            ViewBag.MovieType = items;


            return this.RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult EditTask(int id)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                return this.RedirectToAction("Index");
            }

            var taskDBM = dbContext.TasksB.Find(id);

            if (taskDBM == null)
            {
                return this.RedirectToAction("Index");
            }

            var taskVM = new TaskViewModel();
            taskVM.Id = taskDBM.Id;
            taskVM.Title = taskDBM.Title;
            taskVM.Assigner = taskDBM.Assigner;
            taskVM.Creator = taskDBM.Creator;
            taskVM.Description = taskDBM.Description;
            taskVM.TaskStatus = taskDBM.Status;
            taskVM.Priority = taskDBM.Priority;

            return View(taskVM);
        }

        [HttpPost]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult EditTask(TaskViewModel taskVM)
        {
            if (taskVM.Assigner == "0" || taskVM.Creator == "0" || !ModelState.IsValid)
            {
                return this.View(taskVM);
            }

            dbContext.TasksB
                .Where(t => t.Id == taskVM.Id)
                .Update(t => new TaskB
                {
                    Title = taskVM.Title,
                    Description = taskVM.Description,
                    Creator = taskVM.Creator,
                    Assigner = taskVM.Assigner,
                    Status = taskVM.TaskStatus,
                    Priority = taskVM.Priority
                });

            dbContext.SaveChanges();

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult DeleteTask(int id)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                return this.RedirectToAction("Index");
            }

            var taskDBM = dbContext.TasksB.Find(id);

            if (taskDBM == null)
            {
                return this.RedirectToAction("Index");
            }

            dbContext.TasksB
                .Where(t => t.Id == id)
                .Delete();

            dbContext.SaveChanges();

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "TeamVokil")]
        public ActionResult ChangeTaskStatus(int taskId, string status)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            TaskStatus taskStatus;

            if (status == "tasks")
            {
                taskStatus = TaskStatus.Task;
            }
            else if (status == "in-progress")
            {
                taskStatus = TaskStatus.InProgress;
            }
            else if (status == "fixed")
            {
                taskStatus = TaskStatus.Fixed;
            }
            else if (status == "testing-phase")
            {
                taskStatus = TaskStatus.TestingPhase;
            }
            else if (status == "done")
            {
                taskStatus = TaskStatus.Done;
            }
            else
            {
                throw new ArgumentNullException("Task status cannot be null!");
            }

            dbContext.TasksB.Where(t => t.Id == taskId).Update(t => new TaskB { Status = taskStatus });
            dbContext.SaveChanges();

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Nothing()
        {
            if (User.IsInRole("TeamVokil"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}