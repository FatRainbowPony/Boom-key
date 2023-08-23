using System.Windows;

namespace AppDevTools.Addons
{
    public class WindowInfo
    {
        public class SizeInfo
        {
            #region Properties

            #region Public
            public double Height { get; set; }

            public double Width { get; set; }
            #endregion Public 

            #endregion Properties

            #region Constructors

            #region Public
            public SizeInfo()
            {

            }

            public SizeInfo(double height, double width)
            {
                Height = height;
                Width = width;
            }
            #endregion Public

            #endregion Constructors
        }

        #region Properties

        #region Public
        public Point Location { get; set; }

        public SizeInfo Size { get; set; }

        public WindowState State { get; set; }
        #endregion Public

        #endregion Properties

        #region Constructors

        #region Public
        public WindowInfo()
        {
            Size = new SizeInfo();
        }

        public WindowInfo(Point location, SizeInfo size, WindowState state)
        {
            Location = location;
            Size = size;
            State = state;
        }
        #endregion Public

        #endregion Constructors
    }
}