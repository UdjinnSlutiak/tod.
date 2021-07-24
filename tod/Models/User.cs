using System.Collections.Generic;
using static tod.Models.Topic;

namespace tod.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public double Rating { get; set; }

        public List<Tag> Tags { get; set; } = new();
        //public List<Topic> Topics { get; set; } = new();

    }
}
