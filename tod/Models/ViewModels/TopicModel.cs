using System.Collections.Generic;

namespace tod.Models.ViewModels
{
    public class TopicModel
    {

        public int Id { get; set; }

        public Topic Topic { get; set; }

        public string CommentText { get; set; }

        public int TopicPositiveReactions { get; set; }

        public int TopicNegativeReactions { get; set; }

        //public List<Tag> Tags { get; set; } = new();

    }
}
