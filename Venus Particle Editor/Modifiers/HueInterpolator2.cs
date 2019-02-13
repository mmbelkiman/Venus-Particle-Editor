using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class HueInterpolator2
    {
        private CheckBox checkboxActive;
        private string key = "HueInterpolator2";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private Paragraph colour1H;
        private Slider sliderColour1H;

        private Paragraph colour2H;
        private Slider sliderColour2H;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public HueInterpolator2()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} HUE Interpolator 2"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            panelInside.AddChild(new Paragraph("Color 1", Anchor.AutoInline, new Vector2(0, -1)));
            colour1H = new Paragraph("Hue");
            sliderColour1H = new Slider(0, 360, SliderSkin.Default);
            panelInside.AddChild(colour1H);
            panelInside.AddChild(sliderColour1H);

            panelInside.AddChild(new Paragraph("Color 2", Anchor.AutoInline, new Vector2(0, -1)));
            colour2H = new Paragraph("Hue");
            sliderColour2H = new Slider(0, 360, SliderSkin.Default);
            sliderColour2H.Value = 50;
            panelInside.AddChild(colour2H);
            panelInside.AddChild(sliderColour2H);

        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                sliderColour1H.Value = (int)((VenusParticleEngine.Core.Modifiers.HueInterpolator2)_modifiers[key]).InitialHue;
                sliderColour2H.Value = (int)((VenusParticleEngine.Core.Modifiers.HueInterpolator2)_modifiers[key]).FinalHue;
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.HueInterpolator2());
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
            VenusParticleEngine.Core.Modifiers.HueInterpolator2 currentModifier = (VenusParticleEngine.Core.Modifiers.HueInterpolator2)modifier;

            colour1H.Text = "HUE " + sliderColour1H.Value;
            colour2H.Text = "HUE " + sliderColour2H.Value;

            currentModifier.InitialHue = sliderColour1H.Value;
            currentModifier.FinalHue = sliderColour2H.Value;

            _modifiers[key] = currentModifier;
        }
    }
}
