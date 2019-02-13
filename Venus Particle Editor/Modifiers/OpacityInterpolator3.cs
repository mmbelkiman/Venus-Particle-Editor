using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class OpacityInterpolator3
    {
        private CheckBox checkboxActive;
        private string key = "OpacityInterpolator3";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private Paragraph paragraphInitial;
        private Slider sliderInitial;
        private Paragraph paragraphMedium;
        private Slider sliderMedium;
        private Paragraph paragraphFinal;
        private Slider sliderFinal;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public OpacityInterpolator3()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Opacity Interpolator 3"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            paragraphInitial = new Paragraph("Initial 0%");
            sliderInitial = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(paragraphInitial);
            panelInside.AddChild(sliderInitial);

            paragraphMedium = new Paragraph("Medium 0%");
            sliderMedium = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(paragraphMedium);
            panelInside.AddChild(sliderMedium);

            paragraphFinal = new Paragraph("Final 0%");
            sliderFinal = new Slider(0, 100, SliderSkin.Default);
            panelInside.AddChild(paragraphFinal);
            panelInside.AddChild(sliderFinal);
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                sliderInitial.Value = ((int)(((VenusParticleEngine.Core.Modifiers.OpacityInterpolator3)_modifiers[key]).InitialOpacity * 100));
                sliderMedium.Value = ((int)(((VenusParticleEngine.Core.Modifiers.OpacityInterpolator3)_modifiers[key]).MediumOpacity * 100));
                sliderFinal.Value = ((int)(((VenusParticleEngine.Core.Modifiers.OpacityInterpolator3)_modifiers[key]).FinalOpacity * 100));
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.OpacityInterpolator3());
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
            VenusParticleEngine.Core.Modifiers.OpacityInterpolator3 currentModifier = (VenusParticleEngine.Core.Modifiers.OpacityInterpolator3)modifier;

            currentModifier.InitialOpacity = (float)sliderInitial.Value / 100;
            currentModifier.MediumOpacity = (float)sliderMedium.Value / 100;
            currentModifier.FinalOpacity = (float)sliderFinal.Value / 100;

            paragraphInitial.Text = "Initial " +sliderInitial.Value.ToString() +"%";
            paragraphMedium.Text = "Medium " + sliderMedium.Value.ToString() +"%";
            paragraphFinal.Text = "Final " + sliderFinal.Value.ToString() +"%";

            _modifiers[key] = currentModifier;
        }
    }
}
