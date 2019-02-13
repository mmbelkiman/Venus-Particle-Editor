using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class ColourInterpolator2
    {
        private CheckBox checkboxActive;
        private string key = "ColourInterpolator2";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 750);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private Paragraph colour1H;
        private Slider sliderColour1H;
        private Paragraph colour1S;
        private Slider sliderColour1S;
        private Paragraph colour1L;
        private Slider sliderColour1L;

        private Paragraph colour2H;
        private Slider sliderColour2H;
        private Paragraph colour2S;
        private Slider sliderColour2S;
        private Paragraph colour2L;
        private Slider sliderColour2L;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public ColourInterpolator2()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 600), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Colour Interpolator 2"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            panelInside.AddChild(new Paragraph("Color 1", Anchor.AutoInline, new Vector2(0, -1)));
            colour1H = new Paragraph("Hue");
            sliderColour1H = new Slider(0, 360, SliderSkin.Default);
            panelInside.AddChild(colour1H);
            panelInside.AddChild(sliderColour1H);

            colour1S = new Paragraph("Saturation");
            sliderColour1S = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(colour1S);
            panelInside.AddChild(sliderColour1S);

            colour1L = new Paragraph("Lightness");
            sliderColour1L = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(colour1L);
            panelInside.AddChild(sliderColour1L);

            panelInside.AddChild(new Paragraph("Color 2", Anchor.AutoInline, new Vector2(0, -1)));
            colour2H = new Paragraph("Hue");
            sliderColour2H = new Slider(0, 360, SliderSkin.Default);
            sliderColour2H.Value = 50;
            panelInside.AddChild(colour2H);
            panelInside.AddChild(sliderColour2H);

            colour2S = new Paragraph("Saturation");
            sliderColour2S = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(colour2S);
            panelInside.AddChild(sliderColour2S);

            colour2L = new Paragraph("Lightness");
            sliderColour2L = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(colour2L);
            panelInside.AddChild(sliderColour2L);
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                sliderColour1H.Value = (int)((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).InitialColour.H;
                sliderColour1S.Value = ((int)(((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).InitialColour.S * 100));
                sliderColour1L.Value = ((int)(((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).InitialColour.L * 100));

                sliderColour2H.Value = (int)((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).FinalColour.H;
                sliderColour2S.Value = ((int)(((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).FinalColour.S * 100));
                sliderColour2L.Value = ((int)(((VenusParticleEngine.Core.Modifiers.ColourInterpolator2)_modifiers[key]).FinalColour.L * 100));
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.ColourInterpolator2());
            }
            else
            {
                _modifiers.Remove(key);
            }
        }

        public void Update()
        {
            if (_modifiers == null) { return; }

            if (_modifiers.ContainsKey(key))
            {
                panelInside.Visible = true;
                panelMaster.Size = masterSize;
                checkboxActive.Checked = true;
            }
            else
            {
                panelInside.Visible = false;
                panelMaster.Size = masterSizeMinimized;
                checkboxActive.Checked = false;
            }

            if (_modifiers.ContainsKey(key))
            {
                UpdateModifier(_modifiers[key]);
            }
        }

        private void UpdateModifier(IModifier modifier)
        {
            VenusParticleEngine.Core.Modifiers.ColourInterpolator2 currentModifier = (VenusParticleEngine.Core.Modifiers.ColourInterpolator2)modifier;

            colour1H.Text = "HUE " + sliderColour1H.Value;
            colour1S.Text = "Saturation " + sliderColour1S.Value + "%";
            colour1L.Text = "Lightness " + sliderColour1L.Value + "%";
            colour2H.Text = "HUE " + sliderColour2H.Value;
            colour2S.Text = "Saturation " + sliderColour2S.Value + "%";
            colour2L.Text = "Lightness " + sliderColour2L.Value + "%";

            currentModifier.InitialColour = new VenusParticleEngine.Core.Colour(sliderColour1H.Value, (float)sliderColour1S.Value / 100, (float)sliderColour1L.Value / 100);
            currentModifier.FinalColour = new VenusParticleEngine.Core.Colour(sliderColour2H.Value, (float)sliderColour2S.Value / 100, (float)sliderColour2L.Value / 100);

            _modifiers[key] = currentModifier;
        }
    }
}
