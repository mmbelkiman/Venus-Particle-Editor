using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class Rotation
    {
        private CheckBox checkboxActive;
        private string key = "Rotation";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput _inputRotationRate;


        public Panel Panel
        {
            get { return panelMaster; }
        }

        public Rotation()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Rotation"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            _inputRotationRate = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
            _inputRotationRate.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            _inputRotationRate.Value = "0.1";
            _inputRotationRate.ValueWhenEmpty = "0.1";
            panelInside.AddChild(_inputRotationRate);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                _inputRotationRate.Value = ((RotationModifier)_modifiers[key]).RotationRate.ToString();
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.RotationModifier());
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
            RotationModifier currentModifier = (RotationModifier)modifier;

            if (_inputRotationRate.Value != null && !_inputRotationRate.Value.Equals("") && !_inputRotationRate.Value.Equals("-"))
                currentModifier.RotationRate = float.Parse(_inputRotationRate.Value.Replace(".", ","));

            _modifiers[key] = currentModifier;
        }
    }
}
