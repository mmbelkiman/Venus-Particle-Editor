#region Using Statements 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
#endregion

namespace VenusParticleEditor
{
    public class KeyboardHelper
    {
        public bool Enabled = true;

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public KeyboardHelper()
        {
            KeyboardState = new KeyboardState();
            PreviousKeyboardState = new KeyboardState();
        }

        public KeyboardState KeyboardState { get; private set; }
        public KeyboardState PreviousKeyboardState { get; private set; }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!Enabled) { return; }

            PreviousKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
        }

        public Keys[] GetPressedKeys()
        {
            if (!Enabled) { return new Keys[0]; }

            return KeyboardState.GetPressedKeys();
        }

        public bool IsKeyDown(Keys key)
        {
            if (!Enabled) { return false; }
            Console.WriteLine(key + " " + KeyboardState.IsKeyDown(key) + Keyboard.GetState().IsKeyDown(key));

            return KeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            if (!Enabled) { return false; }

            return KeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
        }

        public bool IsNewKeyRelease(Keys key)
        {
            if (!Enabled) { return false; }

            return PreviousKeyboardState.IsKeyDown(key) && KeyboardState.IsKeyUp(key);
        }
    }
}