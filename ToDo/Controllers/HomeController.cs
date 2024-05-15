using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using ToDo.Models;
using ToDo.Models.ViewModels;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }

        internal ToDoViewModel GetAllTodos()
        {
            List<ToDoItem> todoList = new();

            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=todo;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM todo";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                 new ToDoItem
                                 {
                                     Id = reader.GetInt32(0),
                                     Name = reader.GetString(1)
                                 });
                        }
                    }
                    else
                    {
                        return new ToDoViewModel
                        {
                            ToDoList = todoList
                        };
                    }
                }
            }

            return new ToDoViewModel
            {
                ToDoList = todoList
            };
        }

        public RedirectResult Insert(ToDoItem todo)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=todo;Trusted_Connection=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"INSERT INTO todo (Name) VALUES ('{todo.Name}')";
            SqlCommand command = new(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Records Inserted Successfully");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error Generated. Details: " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return Redirect("https://localhost:7077/");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=todo;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"DELETE from todo WHERE Id = '{id}'";
                SqlCommand command = new(query, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Строка удалена");
            }
            return Json(new { });
        }

        internal ToDoItem GetById(int id)
        {
            ToDoItem todo = new();

            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=todo;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM todo Where Id = '{id}'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);

                    }
                    else
                    {
                        return todo;
                    }
                }
            }

            return todo;
        }

        public RedirectResult Update(ToDoItem todo)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=todo;Trusted_Connection=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
            SqlCommand command = new(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error Generated. Details: " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }

            return Redirect("https://localhost:7077/");
        }

        [HttpGet]
        public JsonResult PopulateForm(int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }
    }
}