namespace System.Drawing
{
    public sealed class Pen
    {
        private Color _color;

        public Pen(Color color)
        {
            this._color = color;
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}
