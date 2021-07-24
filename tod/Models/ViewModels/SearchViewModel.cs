using System.Collections.Generic;

namespace tod.Models.ViewModels
{
    public class SearchViewModel
    {

        public string Title { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }

        public List<Topic> Topics { get; set; } = new();

    }
}
