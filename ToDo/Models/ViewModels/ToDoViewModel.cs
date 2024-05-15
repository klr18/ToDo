namespace ToDo.Models.ViewModels
{
    public class ToDoViewModel
    {
        public required List<ToDoItem> ToDoList { get; set; }
        public ToDoItem ToDo { get; set; }
    }
}
