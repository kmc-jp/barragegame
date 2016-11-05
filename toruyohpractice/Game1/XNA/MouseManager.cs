using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CommonPart
{
    class MouseManager
    {
        MouseState now;
        MouseState old;
        #region singleton
        public static MouseManager mouse_manager = new MouseManager();
        static MouseManager() { }
        private MouseManager() { }
        #endregion
        public void Update_()
        {
            old = now;
            now = Mouse.GetState();
        }

        public int MouseWheelValue()
        {
            return now.ScrollWheelValue;
        }
        public Vector MousePosition()
        {
            return new Vector(now.X, now.Y);
        }
        public Vector OldMousePosition()
        {
            return new Vector(old.X, old.Y);
        }
        /// <summary>
        /// is left/right buttom Down or not
        /// </summary>
        public bool IsButtomDown(MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return ButtonState.Pressed == now.LeftButton;
                case MouseButton.Right:
                    return ButtonState.Pressed == now.RightButton;
                case MouseButton.Middle:
                    return ButtonState.Pressed == now.MiddleButton;
                default:
                    return false;

            }
        }
        /// <summary>
        /// was left/right buttom Down or not before
        /// </summary>
        public bool IsOldButtomDown(MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return ButtonState.Pressed == old.LeftButton;
                case MouseButton.Right:
                    return ButtonState.Pressed == old.RightButton;
                case MouseButton.Middle:
                    return ButtonState.Pressed == old.MiddleButton;
                default:
                    return false;

            }
        }
        public bool IsButtomDownOnce(MouseButton b)
        {
            return IsButtomDown(b) && !IsOldButtomDown(b);
        }
    }// class MouseManager end
}// namespace end
