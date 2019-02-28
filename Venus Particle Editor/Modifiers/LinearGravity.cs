using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class LinearGravity
    {
        private CheckBox checkboxActive;
        private string key = "LinearGravity";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput directionX;
        private TextInput directionY;
        private TextInput strength;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public LinearGravity()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Linear Gravity"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.AddChild(new Paragraph("Direction", Anchor.AutoInline, new Vector2(.9f, -1)));

            panelInside.AddChild(new Paragraph("X", Anchor.AutoInline, new Vector2(0.2f, -1)));
            directionX = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            directionX.Validators.Add(new TextValidatorNumbersOnly(true));
            directionX.Value = "0.1";
            directionX.ValueWhenEmpty = "0.1";
            panelInside.AddChild(directionX);

            panelInside.AddChild(new Paragraph("Y", Anchor.AutoInline, new Vector2(0.2f, -1)));
            directionY = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            directionY.Validators.Add(new TextValidatorNumbersOnly(true));
            directionY.Value = "0.1";
            directionY.ValueWhenEmpty = "0.1";
            panelInside.AddChild(directionY);

            panelInside.AddChild(new Paragraph("Strength", Anchor.AutoInline, new Vector2(.9f, -1)));
            strength = new TextInput(false, new Vector2(0.9f, -1f), anchor: Anchor.AutoInline);
            strength.Validators.Add(new TextValidatorNumbersOnly(true));
            strength.Value = "100";
            strength.ValueWhenEmpty = "0.1";
            panelInside.AddChild(strength);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                directionX.Value = ((LinearGravityModifier)_modifiers[key]).Direction.X.ToString();
                directionY.Value = ((LinearGravityModifier)_modifiers[key]).Direction.Y.ToString();
                strength.Value = ((LinearGravityModifier)_modifiers[key]).Strength.ToString();
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new LinearGravityModifier());
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
            LinearGravityModifier currentModifier = (LinearGravityModifier)modifier;

            if (directionX.Value != null && !directionX.Value.Equals("") && !directionX.Value.Equals("-")
                && directionY.Value != null && !directionY.Value.Equals("") && !directionY.Value.Equals("-"))
                currentModifier.Direction = new Axis(float.Parse(directionX.Value.Replace(".", ",")), float.Parse(directionY.Value.Replace(".", ",")));

            if (strength.Value != null && !strength.Value.Equals("") && !strength.Value.Equals("-"))
                currentModifier.Strength = float.Parse(strength.Value.Replace(".", ","));

            _modifiers[key] = currentModifier;
        }
    }
}
