using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tod.Models.ViewModels
{
    public class ProfileModel
    {
        public User User { get; set; }
        public string TagsTexts { get; set; }
        public string NewName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }

        public List<Tag> Tags { get; set; } = new();
    }
}
