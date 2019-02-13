using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using System;

namespace VenusParticleEditor
{
    public class LoggerHelper
    {
        public enum CONSOLE_TYPE_MESSAGE { ERROR, WARNING, INFO }

        private readonly int PANEL_HEIGHT = 180;

        private RichParagraph _paragraphConsole;
        private Panel _panelConsole;
        private string _consoleTextBackup = "";
        private bool _active = true;

        public bool Active
        {
            set
            {
                _active = value;
                _panelConsole.Visible = _active;
            }

            get
            {
                return _active;
            }
        }
        public Panel PanelConsole
        {
            get { return _panelConsole; }
        }

        public LoggerHelper()
        {
            _panelConsole = new Panel(new Vector2(0.7f, PANEL_HEIGHT), PanelSkin.Simple, Anchor.BottomLeft)
            {
                Padding = Vector2.Zero,
                PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll
            };
            _panelConsole.Scrollbar.AdjustMaxAutomatically = true;

            _paragraphConsole = new RichParagraph("teste", scale: 0.97f);
            _panelConsole.AddChild(_paragraphConsole);
        }

        public void Error(string text)
        {
            WriteConsole(text, CONSOLE_TYPE_MESSAGE.ERROR);
        }

        public void Warning(string text)
        {
            WriteConsole(text, CONSOLE_TYPE_MESSAGE.WARNING);
        }

        public void Info(string text)
        {
            WriteConsole(text, CONSOLE_TYPE_MESSAGE.INFO);
        }

        public void WriteConsole(string text, CONSOLE_TYPE_MESSAGE typeMessage)
        {
            Console.WriteLine(typeMessage.ToString() + " " + text);

            string prefix = "";
            string posfix = "\n";
            switch (typeMessage)
            {
                case CONSOLE_TYPE_MESSAGE.INFO:
                    prefix = "{{BLUE}} - ";
                    break;
                case CONSOLE_TYPE_MESSAGE.WARNING:
                    prefix = "{{ORANGE}} - ";
                    break;
                case CONSOLE_TYPE_MESSAGE.ERROR:
                    prefix = "{{RED}} - ";
                    break;
            }

            if (typeMessage == CONSOLE_TYPE_MESSAGE.ERROR)
            {
                _consoleTextBackup += prefix + text + "{{DEFAULT}}" + posfix;
            }
            else
            {
                _consoleTextBackup += prefix + "{{DEFAULT}}" + text + posfix;
            }

            _paragraphConsole.Text = _consoleTextBackup;
        }
    }
}
