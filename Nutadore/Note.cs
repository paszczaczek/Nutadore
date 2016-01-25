namespace Nutadore
{
    class Note : Sign
    {
        public Note()
        {
        }

        override public double Show(Score score, double left, double top)
        {
            base.code = "\x0055";
            top -= 57.5 * score.Magnification;
            double right = base.Show(score, left, top);

            return right;
        }

    }
}
