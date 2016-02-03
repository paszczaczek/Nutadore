using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
    public class Chord : Sign
    {
        public List<Note> notes = new List<Note>();

        public void Add(Note note)
        {
            notes.Add(note);
        }
    }
}
