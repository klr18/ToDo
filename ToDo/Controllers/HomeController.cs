using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System;
using System.Diagnostics;
using ToDo;
using ToDo.Models;
using ToDo.Models.ViewModels;
using Azure;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace ToDo.Controllers
{
    public interface IDateService
    {
        string GetDate();
    }

    public class RussianDate : IDateService
    {
        public string GetDate()
        {
            DateTime now = DateTime.Now;
            return now.ToString("dd-MM-yyyy hh:mm");
        }
    }

    public class AmericanDate : IDateService
    {
        public string GetDate()
        {
            DateTime now = DateTime.Now;
            return now.ToString("MM-dd-yyyy hh:mm");
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDateService _dateService;
        public HomeController(ILogger<HomeController> logger, IDateService dateService)
        {
            _logger = logger;
            _dateService = dateService;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Executing Index action"); //логирование
            var todoListViewModel = await GetAllTodos();
            var date = _dateService.GetDate();
            ViewData["date"] = date;
            return View(todoListViewModel);
        }

        public string GetDate()
        {
            return _dateService.GetDate();
        }

        public string GetTime()
        {
            return DateTime.Now.ToShortTimeString();
        }

        internal async Task<ToDoViewModel> GetAllTodos()
        {
            await using var db = new ApplicationDbContext();
            var todoList = await db.ToDos.ToListAsync();
            return new ToDoViewModel
            {
                ToDoList = todoList
            };
        }

        public async Task<RedirectResult> Insert(ToDoItem todo)
        {
            await using var db = new ApplicationDbContext();
            _logger.LogInformation("Executing Insert action");

            await db.ToDos.AddRangeAsync(todo);
            await db.SaveChangesAsync();

            return Redirect("https://localhost:7077/");
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            _logger.LogInformation("Executing Delete action");

            await using var db = new ApplicationDbContext();
            var todo = db.ToDos.First(x => x.Id == id);
            db.Remove(todo);
            await db.SaveChangesAsync();

            return Json(new { });
        }

        internal async Task<ToDoItem> GetById(int id)
        {
            await using var db = new ApplicationDbContext();
            var todo = db.ToDos.First(x => x.Id == id);
            return todo;
        }

        public async Task<RedirectResult> Update(ToDoItem todo)
        {
            _logger.LogInformation("Executing Update action");

            await using var db = new ApplicationDbContext();
            var toDo = db.ToDos.First(x => x.Id == todo.Id);
            toDo.Name = todo.Name;
            db.ToDos.Update(toDo);
            await db.SaveChangesAsync();

            return Redirect("https://localhost:7077/");
        }

        [HttpGet]
        public async Task<JsonResult> PopulateForm(int id)
        {
            var todo = await GetById(id);
            return Json(todo);
        }

        public async Task<IActionResult> Download()
        {
            var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
                return BadRequest("File is not selected");

            var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
            // создаем папку для хранения файлов
            Directory.CreateDirectory(uploadPath);

            string fullPath = $"{uploadPath}/{file.FileName}";

            
            // сохраняем файл в папку uploads
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            

            return Ok(file.FileName + " uploaded successfully " + "file location is " + uploadPath);
        }
    }

}