using System;
using System.Collections.Generic;

namespace tod.Models
{

    public class Topic
    {

        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        public List<Commentary> Commentaries { get; set; } = new List<Commentary>();
        public List<Tag> Tags { get; set; } = new();
        public List<TopicReaction> Reactions { get; set; } = new List<TopicReaction>();

    }
}
