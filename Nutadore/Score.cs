﻿using System;
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
        public Scale scale = new Scale(Scale.Based.C, Scale.Type.Major);
		private List<StaffGrand> staffGrands = new List<StaffGrand>();
        public List<Sign> signs = new List<Sign>();

        public Score(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.ClipToBounds = true;
        }

        public void Add(Sign sign)
        {
            if (sign is Chord)
                CorrectPerform(sign as Chord);
            signs.Add(sign);
        }

        private static void CorrectPerform(Chord chord)
        {
            //return;

            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii wilonowej.
            var trebleNotes = chord.notes.FindAll(note => note.staffType == Staff.Type.Treble);

            // Czy sa jakieś nuty wymagające zmiany wysokości wykonania?
            if (trebleNotes.Any(note => note.perform == Note.Perform.TwoOctaveHigher))
            {
                // Sa nuty wymagające zmiany wysokości wykonania o dwie oktawy wyżej.
                // Wyszukaj wszystkie nuty wymagające wykonania o jedną oktawę wyżej
                // i zmień je na wymagające wykonania o dwie oktawy wyżej.
                trebleNotes
                    .FindAll(note => note.perform == Note.Perform.OneOctaveHigher)
                    .ForEach(note =>
                    {
                        note.perform = Note.Perform.TwoOctaveHigher;
                        note.staffPosition.LineNumber -= 3.5;
                    });

                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o dwie oktawy wyżej.
                trebleNotes
                    .FindAll(note => note.perform == Note.Perform.AtPlace)
                    .ForEach(note =>
                    {
                        note.perform = Note.Perform.TwoOctaveHigher;
                        note.staffPosition.LineNumber -= 3.5 * 2;
                    });
            }
            else if (trebleNotes.Any(note => note.perform == Note.Perform.OneOctaveHigher))
            {
                // Są nuty wymagające zmiany wysokści wykonania o oktawę wyżej.
                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o jedną oktawę wyżej.
                trebleNotes
                    .FindAll(note => note.perform == Note.Perform.AtPlace)
                    .ForEach(note =>
                    {
                        note.perform = Note.Perform.OneOctaveHigher;
                        note.staffPosition.LineNumber -= 3.5;
                    });
            }

            // Wyszukaj wszystkie nuty akordu leżące na pięcilinii basowej.
            var bassNotes = chord.notes.FindAll(note => note.staffType == Staff.Type.Bass);

            // Czy sa jakieś nuty wymagające zmiany wysokości wykonania?
            if (bassNotes.Any(note => note.perform == Note.Perform.OneOctaveLower))
            {
                // Jest przynajmniej jedna nuta wymagająca wykonania o oktawę niżej.
                // Wyszukaj wszystkie nuty nie wymagające zmiany wykonania i zmień je na
                // wymagające wykonania o jedną oktawę niżej.
                bassNotes
                    .FindAll(note => note.perform == Note.Perform.AtPlace)
                    .ForEach(note =>
                    {
                        note.perform = Note.Perform.OneOctaveLower;
                        note.staffPosition.LineNumber += 3.5;
                    });
            }
        }

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
			double top = 0;
			while (true)
			{
				StaffGrand staffGrand = new StaffGrand(this, top);
				staffGrands.Add(staffGrand);
				top = staffGrand.bottom;
				if (staffGrand.allSignsFitted)
					break;
			}
		}

		public void Clear()
        {
            // usuwamy wszystkie nuty
            signs.ForEach(sign => sign.Hide(this));

            // usuwamy pozostałe elemetny (klucze, znaki przykluczowe, itd.)
            canvas.Children.Clear();

			// usuwamy wszystkie podwójne pięciolinie
			staffGrands.Clear();
        }
    }
}
