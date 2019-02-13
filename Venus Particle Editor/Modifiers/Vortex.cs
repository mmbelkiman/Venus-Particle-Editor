using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class Vortex
    {
        private CheckBox checkboxActive;
        private string key = "Vortex";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 500);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput positionX;
        private TextInput positionY;
        private TextInput mass;
        private TextInput maxSpeed { get; set; }

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public Vortex()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 380), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };
            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Vortex"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.AddChild(new Paragraph("Position", Anchor.AutoInline, new Vector2(.9f, -1)));

            panelInside.AddChild(new Paragraph("X", Anchor.AutoInline, new Vector2(0.2f, -1)));
            positionX = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            positionX.Validators.Add(new TextValidatorNumbersOnly(true));
            positionX.Value = "50";
            positionX.ValueWhenEmpty = "0.1";
            panelInside.AddChild(positionX);

            panelInside.AddChild(new Paragraph("Y", Anchor.AutoInline, new Vector2(0.2f, -1)));
            positionY = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            positionY.Validators.Add(new TextValidatorNumbersOnly(true));
            positionY.Value = "50";
            positionY.ValueWhenEmpty = "0.1";
            panelInside.AddChild(positionY);

            panelInside.AddChild(new Paragraph("Mass", Anchor.AutoInline, new Vector2(.9f, -1)));
            mass = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            mass.Validators.Add(new TextValidatorNumbersOnly(true));
            mass.Value = "10";
            mass.ValueWhenEmpty = "0.1";
            panelInside.AddChild(mass);

            panelInside.AddChild(new Paragraph("Max Speed", Anchor.AutoInline, new Vector2(.9f, -1)));
            maxSpeed = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            maxSpeed.Validators.Add(new TextValidatorNumbersOnly(true));
            maxSpeed.Value = "10";
            maxSpeed.ValueWhenEmpty = "0.1";
            panelInside.AddChild(maxSpeed);
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                positionX.Value = ((VortexModifier)_modifiers[key]).Position.X.ToString();
                positionY.Value = ((VortexModifier)_modifiers[key]).Position.Y.ToString();
                mass.Value = ((VortexModifier)_modifiers[key]).Mass.ToString();
                maxSpeed.Value = ((VortexModifier)_modifiers[key]).MaxSpeed.ToString();
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.VortexModifier());
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
            VortexModifier currentModifier = (VortexModifier)modifier;

            if (positionX.Value != null && !positionX.Value.Equals("") && !positionX.Value.Equals("-")
                 && positionY.Value != null && !positionY.Value.Equals("") && !positionY.Value.Equals("-"))
                currentModifier.Position = new Vector(float.Parse(positionX.Value.Replace(".", ",")), float.Parse(positionY.Value.Replace(".", ",")));

            if (mass.Value != null && !mass.Value.Equals("") && !mass.Value.Equals("-"))
                currentModifier.Mass = float.Parse(mass.Value.Replace(".", ","));

            if (maxSpeed.Value != null && !maxSpeed.Value.Equals("") && !maxSpeed.Value.Equals("-"))
                currentModifier.MaxSpeed = float.Parse(maxSpeed.Value.Replace(".", ","));

            _modifiers[key] = currentModifier;
        }
    }
}
