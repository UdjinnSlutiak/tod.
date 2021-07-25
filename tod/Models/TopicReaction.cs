using System;
namespace tod.Models
{
    public class TopicReaction
    {

        public int Id { get; set; }
        public User ReactedUser { get; set; }
        public int Value { get; set; }

    }
}
