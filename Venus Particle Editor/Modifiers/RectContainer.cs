using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using VenusParticleEngine.Core.Modifiers.Container;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class RectContainer
    {
        private CheckBox _checkboxActive;
        private string _key = "RectContainerModifier";
        private Panel _panelMaster;
        private Panel _panelInside;
        private Vector2 _masterSize = new Vector2(-1, 450);
        private Vector2 _masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private Paragraph _paragraphWidth;
        private TextInput _inputWidth;
        private Paragraph _paragraphHeight;
        private TextInput _inputHeight;

        private Paragraph _paragraphRestitutionCoefficient;
        private TextInput _inputRestitutionCoefficient;

        public Panel Panel
        {
            get { return _panelMaster; }
        }

        public RectContainer()
        {
            _panelMaster = new Panel(_masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            _panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 330), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            _checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            _panelMaster.AddChild(new RichParagraph("{{GOLD}} Rect Container"));
            _panelMaster.AddChild(_checkboxActive);
            _panelMaster.AddChild(_panelInside);

            _paragraphWidth = new Paragraph("Width", size: new Vector2(0.5f, -1), anchor: Anchor.AutoInline);
            _inputWidth = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
            _inputWidth.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: false));
            _inputWidth.Value = "1";
            _inputWidth.ValueWhenEmpty = "1";
            _panelInside.AddChild(_paragraphWidth);
            _panelInside.AddChild(_inputWidth);

            _paragraphHeight = new Paragraph("Height", size: new Vector2(0.5f, -1), anchor: Anchor.AutoInline);
            _inputHeight = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
            _inputHeight.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: false));
            _inputHeight.Value = "1";
            _inputHeight.ValueWhenEmpty = "1";
            _panelInside.AddChild(_paragraphHeight);
            _panelInside.AddChild(_inputHeight);

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
                _inputWidth.Value = ((VenusParticleEngine.Core.Modifiers.Container.RectContainerModifier)_modifiers[_key]).Width.ToString().Replace(",", ".");
                _inputHeight.Value = ((VenusParticleEngine.Core.Modifiers.Container.RectContainerModifier)_modifiers[_key]).Height.ToString().Replace(",", ".");

                _inputRestitutionCoefficient.Value = ((VenusParticleEngine.Core.Modifiers.Container.RectContainerModifier)_modifiers[_key]).RestitutionCoefficient.ToString().Replace(",", ".");
            }
        }

        private void OnClickCheckboxActive()
        {
            if (_checkboxActive.Checked)
            {
                _modifiers.Add(_key, new VenusParticleEngine.Core.Modifiers.Container.RectContainerModifier());
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
            RectContainerModifier currentModifier = (RectContainerModifier)modifier;

            if (_inputWidth.Value != null && !_inputWidth.Value.Equals("") && !_inputWidth.Value.Equals("-"))
                currentModifier.Width = int.Parse(_inputWidth.Value.Replace(".", ","));

            if (_inputHeight.Value != null && !_inputHeight.Value.Equals("") && !_inputHeight.Value.Equals("-"))
                currentModifier.Height = int.Parse(_inputHeight.Value.Replace(".", ","));

            if (_inputRestitutionCoefficient.Value != null && !_inputRestitutionCoefficient.Value.Equals("") && !_inputRestitutionCoefficient.Value.Equals("-"))
                currentModifier.RestitutionCoefficient = float.Parse(_inputRestitutionCoefficient.Value.Replace(".", ","));

            _modifiers[_key] = currentModifier;
        }
    }
}
