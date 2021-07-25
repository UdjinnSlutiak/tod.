using System;
namespace tod.Models
{
    public class TagUser
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }

    }
}
