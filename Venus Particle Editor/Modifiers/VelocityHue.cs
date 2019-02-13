using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class VelocityHue
    {
        private CheckBox checkboxActive;
        private string key = "VelocityHue";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 500);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput stationaryHue;
        private TextInput velocityHue;
        private TextInput velocityThreshold;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public VelocityHue()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 380), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };
            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            panelMaster.AddChild(new RichParagraph("{{GOLD}} VelocityHue"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.AddChild(new Paragraph("Stationary Hue", Anchor.AutoInline, new Vector2(.9f, -1)));
            stationaryHue = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            stationaryHue.Validators.Add(new TextValidatorNumbersOnly(true));
            stationaryHue.Value = "1";
            stationaryHue.ValueWhenEmpty = "0.1";
            panelInside.AddChild(stationaryHue);

            panelInside.AddChild(new Paragraph("Velocity Hue", Anchor.AutoInline, new Vector2(.9f, -1)));
            velocityHue = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            velocityHue.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            velocityHue.Value = "1000";
            velocityHue.ValueWhenEmpty = "0.1";
            panelInside.AddChild(velocityHue);

            panelInside.AddChild(new Paragraph("Velocity Threshold", Anchor.AutoInline, new Vector2(.9f, -1)));
            velocityThreshold = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            velocityThreshold.Validators.Add(new TextValidatorNumbersOnly(true));
            velocityThreshold.Value = "300";
            velocityThreshold.ValueWhenEmpty = "0.1";
            panelInside.AddChild(velocityThreshold);
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                stationaryHue.Value = ((VelocityHueModifier)_modifiers[key]).StationaryHue.ToString();
                velocityHue.Value = ((VelocityHueModifier)_modifiers[key]).VelocityHue.ToString();
                velocityThreshold.Value = ((VelocityHueModifier)_modifiers[key]).VelocityThreshold.ToString();
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.VelocityHueModifier());
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
            VelocityHueModifier currentModifier = (VelocityHueModifier)modifier;

            if (stationaryHue.Value != null && !stationaryHue.Value.Equals("") && !stationaryHue.Value.Equals("-"))
                currentModifier.StationaryHue = float.Parse(stationaryHue.Value.Replace(".", ","));

            if (velocityHue.Value != null && !velocityHue.Value.Equals("") && !velocityHue.Value.Equals("-"))
                currentModifier.VelocityHue = float.Parse(velocityHue.Value.Replace(".", ","));

            if (velocityThreshold.Value != null && !velocityThreshold.Value.Equals("") && !velocityThreshold.Value.Equals("-"))
                currentModifier.VelocityThreshold = float.Parse(velocityThreshold.Value.Replace(".", ","));

            _modifiers[key] = currentModifier;
        }
    }
}
