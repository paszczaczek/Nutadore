\version "2.18.2"

\parallelMusic #'(voiceA voiceB voiceC) {
  % bar 1  
    <fis''-2>1~-\markup{fis2} 
  | <d''-1>1~-\markup{d2} 
  | <d-5>4-\markup{d} d'^\markup{d1} cis'^\markup{cis1} cis-\markup{cis} 
  

  % bar 2  
  | <fis''-2>1~-\markup{fis2}
  | <d''-1>1~-\markup{d2} 
  | <b,-5>4-\markup{B} <b-1>^\markup{b} <a>^\markup{a} <a,-4>-\markup{A}

  % bar 3
  %| <fis''-2>4-\markup{fis2} <b''-5>8-\markup{b2} <g''-3>-\markup{g2} \grace<fis''-5> <e''-4>-\markup{e2} d''-\markup{d2} cis''-\markup{cis2} e''-\markup{e2}
  | <fis''-2>4-\markup{fis2} <b''-5>8-\markup{b2} <g''-3>8-\markup{g2}    <e''-4>8-\markup{e2} d''8-\markup{d2} cis''8-\markup{cis2} e''8-\markup{e2}
  | <d''-1>2-\markup{d2} <b'-1>2-\markup{b1}
  | <g,-5>4-\markup{G} <g-2>4^\markup{g} <gis-1>4^\markup{gis} <gis,-5>4-\markup{Gis} 

  % bar 4
  | <cis''-3>2-\markup{cis2} a'8-\markup{a1} g'4.-\markup{g1} 
  | <a'-1>2-\markup{a1} r2
  | <a,-5>4-\markup{A} <a-1>4^\markup{a} <g>4^\markup{g} <g,-4>4-\markup{G}

  
  % bar 5
  | <a''-5>1~-\markup{a2}
  | r4 <c''-2>8-\markup{c2} <a'-1>8-\markup{a1} <c''>4-\markup{c2} <a''>4-\markup{a2}
  %| f'4 <c''-2>8-\markup{c2} <a'-1>8-\markup{a1} <c''>4-\markup{c2} <a''>4-\markup{a2}
  | <fis,-5>-\markup{Fis} <fis>^\markup{fis} <e>^\markup{e} <e,-4>-\markup{E}


  % bar 6
  | <a''>8-\markup{a2} <g''-4>8-\markup{g2} <c''-2>8-\markup{c2} <b'>8-\markup{b1} <e''>8-\markup{e2} <dis''>8-\markup{dis2} <a''-5>8-\markup{a2} <g''>8-\markup{g2}
  | <b'>4-\markup{b1} r4 r2
  | <dis,-5>4-\markup{Dis} <dis-1>-\markup{dis} <b,>-\markup{B} <g>-\markup{g}
 
  % bar 7
  | <g''-5>1~-\markup{g2}
  | <b'-1>4-\markup{b1} <e''-3>8-\markup{e2} <d''>-\markup{d2}  <e''>-\markup{e2} <fis''>-\markup{fis2} <g''>-\markup{g2} <e''>-\markup{e2}
  | <e,>4-\markup{E} <e>-\markup{e} <d>-\markup{d} <d,-4>-\markup{D}

  % bar 8
  | <g''>8-\markup{g2} <e''-5>8-\markup{e2} <b'-2>8-\markup{g1} <a'>8-\markup{a1} <d''-3>8-\markup{d2} <cis''>8-\markup{c2} <g''-5>8-\markup{g2} <fis''-4>8-\markup{fis2}
  | <a'>4-\markup{a1} r4 r2
  | <cis,-5>4-\markup{Cis} <cis>4-\markup{cis} <a,>4-\markup{A} <fis>4-\markup{fis}  
  

}


\header {
  title = "Air i D major"
  subtitle = "from Orchestral Suite No.3"
  instrument = "Piano"
  composer = "J.S Bach"
  %arranger = "Arrangement by www.Galya.fr"
  meter = "version 1"
}

\score {
  \new PianoStaff <<  
    \new Staff <<            
      \tempo "Adagio"
      \clef treble
      \key d \major
      \voiceA 
      \\ 
       \voiceB
    >>
    \new Staff <<
      \clef bass
      \key d \major
       \voiceC
    >>    
    %{
    \new NoteNames { \set printOctaveNames = ##t \voiceA }
    \new NoteNames { \set printOctaveNames = ##t \voiceB }
    \new NoteNames { \set printOctaveNames = ##t \voiceC }
    %}
  >>
  \layout {
    \set fingeringOrientations = #'(left)
    \override Score.BarNumber.break-visibility = ##(#f #t #t)
  }
  \midi {
  }
}