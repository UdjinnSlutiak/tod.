using System;
namespace tod.Models
{
    public class CommentaryReaction
    {

        public int Id { get; set; }
        public User ReactedUser { get; set; }
        public int Value { get; set; }

    }
}
