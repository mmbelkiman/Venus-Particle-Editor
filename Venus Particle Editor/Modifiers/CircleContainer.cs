using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using VenusParticleEngine.Core.Modifiers.Container;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class CircleContainer
    {
        private CheckBox _checkboxActive;
        private string _key = "CircleContainerModifier";
        private Panel _panelMaster;
        private Panel _panelInside;
        private Vector2 _masterSize = new Vector2(-1f, 450);
        private Vector2 _masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private Paragraph _paragraphRadius;
        private TextInput _inputRadius;

        private CheckBox _checkboxInside;

        private Paragraph _paragraphRestitutionCoefficient;
        private TextInput _inputRestitutionCoefficient;

        public Panel Panel
        {
            get { return _panelMaster; }
        }

        public CircleContainer()
        {
            _panelMaster = new Panel(_masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            _panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 330), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            _checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            _panelMaster.AddChild(new RichParagraph("{{GOLD}} Circle Container"));
            _panelMaster.AddChild(_checkboxActive);
            _panelMaster.AddChild(_panelInside);

            _paragraphRadius = new Paragraph("Radius");
            _inputRadius = new TextInput(false, new Vector2(-1, -1f), anchor: Anchor.AutoInline);
            _inputRadius.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
            _inputRadius.Value = "0.1";
            _inputRadius.ValueWhenEmpty = "0.1";
            _panelInside.AddChild(_paragraphRadius);
            _panelInside.AddChild(_inputRadius);

            _checkboxInside = new CheckBox("Inside", isChecked: false);
            _panelInside.AddChild(_checkboxInside);

            _paragraphRestitutionCoefficient = new Paragraph("Restitution Coefficient");
            _inputRestitutionCoefficient = new TextInput(false, new Vector2(-1, -1f), anchor: Anchor.AutoInline);
            _inputRestitutionCoefficient.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
            _inputRestitutionCoefficient.Value = "0.1";
            _inputRestitutionCoefficient.ValueWhenEmpty = "0.1";
            _panelInside.AddChild(_paragraphRestitutionCoefficient);
            _panelInside.AddChild(_inputRestitutionCoefficient);

            _panelInside.Visible = false;
            _panelMaster.Size = _masterSizeMinimized;
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(_key))
            {
                _inputRadius.Value = ((VenusParticleEngine.Core.Modifiers.Container.CircleContainerModifier)_modifiers[_key]).Radius.ToString().Replace(",", ".");

                _checkboxInside.Checked = ((VenusParticleEngine.Core.Modifiers.Container.CircleContainerModifier)_modifiers[_key]).Inside;

                _inputRestitutionCoefficient.Value = ((VenusParticleEngine.Core.Modifiers.Container.CircleContainerModifier)_modifiers[_key]).RestitutionCoefficient.ToString().Replace(",", ".");
            }
        }

        private void OnClickCheckboxActive()
        {
            if (_checkboxActive.Checked)
            {
                _modifiers.Add(_key, new VenusParticleEngine.Core.Modifiers.Container.CircleContainerModifier());
            }
            else
            {
                _modifiers.Remove(_key);
            }
        }

        public void Update()
        {
            if (_modifiers == null) { return; }

            if (_modifiers.ContainsKey(_key))
            {
                _panelInside.Visible = true;
                _panelMaster.Size = _masterSize;
                _checkboxActive.Checked = true;
            }
            else
            {
                _panelInside.Visible = false;
                _panelMaster.Size = _masterSizeMinimized;
                _checkboxActive.Checked = false;
            }

            if (_modifiers.ContainsKey(_key))
            {
                UpdateModifier(_modifiers[_key]);
            }
        }

        private void UpdateModifier(IModifier modifier)
        {
            CircleContainerModifier currentModifier = (CircleContainerModifier)modifier;

            if (_inputRadius.Value != null && !_inputRadius.Value.Equals("") && !_inputRadius.Value.Equals("-"))
                currentModifier.Radius = float.Parse(_inputRadius.Value.Replace(".", ","));

            currentModifier.Inside = _checkboxInside.Checked;

            if (_inputRestitutionCoefficient.Value != null && !_inputRestitutionCoefficient.Value.Equals("") && !_inputRestitutionCoefficient.Value.Equals("-"))
                currentModifier.RestitutionCoefficient = float.Parse(_inputRestitutionCoefficient.Value.Replace(".", ","));

            _modifiers[_key] = currentModifier;
        }
    }
}
