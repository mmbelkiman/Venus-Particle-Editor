using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class ScaleInterpolator2
    {
        private CheckBox checkboxActive;
        private string key = "ScaleInterpolator2";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput _inputInitialX;
        private TextInput _inputInitialY;
        private TextInput _inputFinalX;
        private TextInput _inputFinalY;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public ScaleInterpolator2()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };
            panelMaster.AddChild(new RichParagraph("{{GOLD}} Scale Interpolator 2"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);
            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;

            panelInside.AddChild(new Paragraph("Initial", Anchor.AutoInline, new Vector2(0, -1)));
            panelInside.AddChild(new Paragraph("X", Anchor.AutoInline, new Vector2(0.2f, -1)));
            _inputInitialX = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            _inputInitialX.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            _inputInitialX.Value = "1";
            _inputInitialX.ValueWhenEmpty = "1";
            panelInside.AddChild(_inputInitialX);

            panelInside.AddChild(new Paragraph("Y", Anchor.AutoInline, new Vector2(0.2f, -1)));
            _inputInitialY = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            _inputInitialY.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            _inputInitialY.Value = "1";
            _inputInitialY.ValueWhenEmpty = "1";
            panelInside.AddChild(_inputInitialY);

            panelInside.AddChild(new Paragraph("Final", Anchor.AutoInline, new Vector2(0, -1)));
            panelInside.AddChild(new Paragraph("X", Anchor.AutoInline, new Vector2(0.2f, -1)));
            _inputFinalX = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            _inputFinalX.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            _inputFinalX.Value = "1.5";
            _inputFinalX.ValueWhenEmpty = "1";
            panelInside.AddChild(_inputFinalX);

            panelInside.AddChild(new Paragraph("Y", Anchor.AutoInline, new Vector2(0.2f, -1)));
            _inputFinalY = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
            _inputFinalY.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            _inputFinalY.Value = "1.5";
            _inputFinalY.ValueWhenEmpty = "1";
            panelInside.AddChild(_inputFinalY);
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                _inputInitialX.Value = ((VenusParticleEngine.Core.Modifiers.ScaleInterpolator2)_modifiers[key]).InitialScale.X.ToString().Replace(",", ".");
                _inputInitialY.Value = ((VenusParticleEngine.Core.Modifiers.ScaleInterpolator2)_modifiers[key]).InitialScale.Y.ToString().Replace(",", ".");

                _inputFinalX.Value = ((VenusParticleEngine.Core.Modifiers.ScaleInterpolator2)_modifiers[key]).FinalScale.X.ToString().Replace(",",".");
                _inputFinalY.Value = ((VenusParticleEngine.Core.Modifiers.ScaleInterpolator2)_modifiers[key]).FinalScale.Y.ToString().Replace(",", ".");
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.ScaleInterpolator2());
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
            VenusParticleEngine.Core.Modifiers.ScaleInterpolator2 currentModifier = (VenusParticleEngine.Core.Modifiers.ScaleInterpolator2)modifier;

            Vector initial = new Vector(0, 0);
            Vector final = new Vector(0, 0);

            if (_inputInitialX.Value != null && !_inputInitialX.Value.Equals("") && !_inputInitialX.Value.Equals("-"))
                initial.X = float.Parse(_inputInitialX.Value.Replace(".", ","));

            if (_inputInitialY.Value != null && !_inputInitialY.Value.Equals("") && !_inputInitialY.Value.Equals("-"))
                initial.Y = float.Parse(_inputInitialY.Value.Replace(".", ","));

            if (_inputFinalX.Value != null && !_inputFinalX.Value.Equals("") && !_inputFinalX.Value.Equals("-"))
                final.X = float.Parse(_inputFinalX.Value.Replace(".", ","));

            if (_inputFinalY.Value != null && !_inputFinalY.Value.Equals("") && !_inputFinalY.Value.Equals("-"))
                final.Y = float.Parse(_inputFinalY.Value.Replace(".", ","));

            currentModifier.InitialScale = initial;
            currentModifier.FinalScale = final;
                                   
            _modifiers[key] = currentModifier;
        }
    }
}
