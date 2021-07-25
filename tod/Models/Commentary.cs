using System;
using System.Collections.Generic;

namespace tod.Models
{
    public class Commentary
    {

        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public Topic Topic { get; set; }
        public DateTime Created { get; set; }

        public List<CommentaryReaction> Reactions { get; set; } = new();

    }
}
