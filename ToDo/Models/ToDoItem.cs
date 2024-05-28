using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToDo.Models
{
    [Table("todo_info", Schema = "public")]
    public class ToDoItem
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name", TypeName = "varchar(100)"), Required]
        public string Name { get; set; }

    }
}
