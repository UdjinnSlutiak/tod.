using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace tod.Models
{
    public class Tag
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }

        public List<Topic> Topics { get; set; } = new();
        public List<User> Users { get; set; } = new();

    }
}
