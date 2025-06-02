using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStart
{
    internal class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }

        // Для хранения выбранного пользователем ответа
        public int? SelectedIndex { get; set; }
    }
}
