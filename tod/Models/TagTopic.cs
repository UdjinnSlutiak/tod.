using System;
namespace tod.Models
{
    public class TagTopic
    {
        public int Id { get; set; }
        public Topic Topic { get; set; }
        public int TopicId { get; set; }
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}
