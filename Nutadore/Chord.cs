using System.Collections.Generic;
using System.Linq;

namespace Nutadore
{
    public class Chord : Sign
    {
        public List<Note> notes = new List<Note>();

        public double left;

        public double right;

        public Perform.HowTo performHowToStaffTreble;
        public Perform.HowTo performHowToStaffBass;

        public void Add(Note note)
        {
            notes.Add(note);
        }

        public override double Show(Score score, Staff trebleStaff, Staff bassStaff, double left)
        {
            CalculatAndCorrectPerformHowTo();

            double chordLeft = left;
            double chordRight = left;
            double cursor = left;
            foreach (var note in notes)
            {
                Staff staff
                    = note.staffType == Staff.Type.Treble
                    ? trebleStaff
                    : bassStaff;
                double noteCursor = note.Show(score, trebleStaff, bassStaff, left);
                if (noteCursor > cursor)
                    cursor = noteCursor;
                if (note.right > chordRight || note.right == -1)
                    chordRight = note.right;
            }

            this.left = chordLeft;
            this.right = chordRight;

            return cursor;
        }

        public override void Hide(Score score)
        {
            notes.ForEach(note => note.Hide(score));
        }

        public override bool IsShown
        {
            get
            {
                return notes.Find(note => note.IsShown) != null;
            }
        }

        public void CalculatAndCorrectPerformHowTo()
        {
            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii wilonowej.
            var trebleNotes = notes.FindAll(note => note.staffType == Staff.Type.Treble);

            // Czy sa jakieś nuty wymagające zmiany wysokości wykonania na pieciolinii wiolinowej?
            if (trebleNotes.Any(note => note.performHowTo == Perform.HowTo.TwoOctaveHigher))
            {
                // Sa nuty wymagające zmiany wysokości wykonania o dwie oktawy wyżej.
                // Wyszukaj wszystkie nuty wymagające wykonania o jedną oktawę wyżej
                // i zmień je na wymagające wykonania o dwie oktawy wyżej.
                trebleNotes
                    .FindAll(note => note.performHowTo == Perform.HowTo.OneOctaveHigher)
                    .ForEach(note =>
                    {
                        note.performHowTo = Perform.HowTo.TwoOctaveHigher;
                        note.staffPosition.Number -= 3.5;
                    });

                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o dwie oktawy wyżej.
                trebleNotes
                    .FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
                    .ForEach(note =>
                    {
                        note.performHowTo = Perform.HowTo.TwoOctaveHigher;
                        note.staffPosition.Number -= 3.5 * 2;
                    });

                // Teraz wszystkie nuty akordu leżące na pięciolinii wiolinowej
                // będą wykonywane o dwie oktawy wyzej.
                performHowToStaffTreble = Perform.HowTo.TwoOctaveHigher;
            }
            else if (trebleNotes.Any(note => note.performHowTo == Perform.HowTo.OneOctaveHigher))
            {
                // Są nuty wymagające zmiany wysokści wykonania o oktawę wyżej.
                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o jedną oktawę wyżej.
                trebleNotes
                    .FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
                    .ForEach(note =>
                    {
                        note.performHowTo = Perform.HowTo.OneOctaveHigher;
                        note.staffPosition.Number -= 0.0; // To ma tak byc!
                    });

                // Teraz wszyskie nuty akordu leżące na pęciolinii wiolinowej będą
                // wykonywane o oktawę wyżej.
                performHowToStaffTreble = Perform.HowTo.OneOctaveHigher;
            }

            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii basowej.
            var bassNotes = notes.FindAll(note => note.staffType == Staff.Type.Bass);

            // Czy sa jakieś nuty wymagające zmiany wysokości wykonania?
            if (bassNotes.Any(note => note.performHowTo == Perform.HowTo.OneOctaveLower))
            {
                // Jest przynajmniej jedna nuta wymagająca wykonania o oktawę niżej.
                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o jedną oktawę niżej.
                bassNotes
                    .FindAll(note => note.performHowTo == Perform.HowTo.AtPlace)
                    .ForEach(note =>
                    {
                        note.performHowTo = Perform.HowTo.OneOctaveLower;
                        note.staffPosition.Number += 3.5;
                    });

                // Teraz wszystkie nuty leżące na pięciolinii basowej
                // będą wykonywane o oktawę nizej.
                performHowToStaffBass = Perform.HowTo.OneOctaveLower;
            }
        }
    }
}
