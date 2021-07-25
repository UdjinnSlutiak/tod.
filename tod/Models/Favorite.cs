using System;
namespace tod.Models
{
    public class Favorite
    {

        public int Id { get; set; }
        public User User { get; set; }
        public Topic Topic { get; set; }

    }
}
