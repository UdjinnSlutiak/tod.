using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tod.Models.ViewModels
{
    public class CreateModel
    {

        public int Id { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Topic title have to be from 10 to 300 symbols")]
        public string Text { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();
        public string TagsTexts { get; set; }

        public Topic Topic { get; set; }

    }
}
