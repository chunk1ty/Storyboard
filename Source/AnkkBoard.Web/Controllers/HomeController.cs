﻿namespace AnkkBoard.Web.Controllers
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
        // Get all tasks
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
                        TaskStatus = x.Status
                    })
                  .ToList();

            return View(allTasks);
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
            return View();
        }
    }
}