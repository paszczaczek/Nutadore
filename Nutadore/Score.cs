using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nutadore
{
    public class Score
    {
        public Canvas canvas;
        public Scale scale = new Scale(Note.Letter.C, Scale.Type.Major);
        private List<StaffGrand> staffGrands = new List<StaffGrand>();
        public List<Sign> signs = new List<Sign>();

        public Score(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.ClipToBounds = true;
            Magnification = Properties.Settings.Default.ScoreMagnification;
        }

        public void Add(Sign sign)
        {
            //if (sign is Chord)
            //    CorrectPerform(sign as Chord);
            signs.Add(sign);
        }

#if false
        // wywalic
        private static void CorrectPerform(Chord chord)
        {
            //return;

            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii wilonowej.
            var trebleNotes = chord.notes.FindAll(note => note.staffType == Staff.Type.Treble);

            // Czy sa jakieś nuty wymagające zmiany wysokości wykonania?
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
                        note.staffPosition.Number -= 3.5;
                    });
            }

            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii basowej.
            var bassNotes = chord.notes.FindAll(note => note.staffType == Staff.Type.Bass);

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
            }
        }
#endif

        public Note FindNextNote(Sign sign)
        {
            int idx = signs.IndexOf(sign);
            int idxNextNote = signs.FindIndex(idx, s => s is Note);

            return signs[idxNextNote] as Note;
        }

        private double magnification = 1.0;
        public double Magnification {
            get
            {
                return magnification;
            }
            set
            {
                magnification = value;
                Show();
            }
        }

        public void Show()
        {
            // Czyscimy partyturę.
            Clear();

            // Rysujemy tyle podwójnych pięciolinii, ile potrzeba
            // aby zmieściły się na nich wszystkie znaki.
            double staffGrandTop = 0;
            bool allSignsIsShown = false;
            Sign fromSign = signs.FirstOrDefault();
            while (!allSignsIsShown)
            {
                // Rysujemy nowy StaffGrand.
                StaffGrand staffGrand = new StaffGrand(this, staffGrandTop);
                staffGrands.Add(staffGrand);

                // Wyświetlamy na nim znaki.
                Sign nextSign;
                staffGrandTop = staffGrand.Show(fromSign, out nextSign);
                if (nextSign == fromSign)
                {
                    // Żadnej nuty nie udało się narysować (nie zmieścił się żaden takt).
                    // Za wąska partytura - przerywany rysowanie.
                    break;
                }
                fromSign = nextSign;

                // Czy wszystkie znaki zmieściły się na nim?
                allSignsIsShown = staffGrand.lastSign == signs.Last();
            }
        }

        public void Clear()
        {
            // usuwamy wszystkie nuty
            signs.ForEach(sign => sign.Hide(this));

            // usuwamy wszystkie podwójne pięciolinie
            foreach (var staffGrand in staffGrands)
                staffGrand.Hide();
            staffGrands.Clear();

            // usuwamy pozostałe elemetny (klucze, znaki przykluczowe, itd.)
            canvas.Children.Clear();
        }
    }
}
