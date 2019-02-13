using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class Drag
    {
        private CheckBox checkboxActive;
        private string key = "Drag";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 400);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        private TextInput dragCoefficient;
        private TextInput density;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public Drag()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 250), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };
            panelMaster.AddChild(new RichParagraph("{{GOLD}} Drag"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.AddChild(new Paragraph("Drag Coefficient", Anchor.AutoInline, new Vector2(0.5f, -1)));
            dragCoefficient = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
            dragCoefficient.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            dragCoefficient.Value = "1.5";
            dragCoefficient.ValueWhenEmpty = "0.1";
            panelInside.AddChild(dragCoefficient);

            panelInside.AddChild(new Paragraph("Density", Anchor.AutoInline, new Vector2(0.5f, -1)));
            density = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
            density.Validators.Add(new TextValidatorNumbersOnly(true, 0));
            density.Value = "80.1";
            density.ValueWhenEmpty = "0.1";
            panelInside.AddChild(density);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;

            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
                dragCoefficient.Value = ((DragModifier)_modifiers[key]).DragCoefficient.ToString();
                density.Value = ((DragModifier)_modifiers[key]).Density.ToString();
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new DragModifier());
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
            DragModifier currentModifier = (DragModifier)modifier;

            if (dragCoefficient.Value != null && !dragCoefficient.Value.Equals("") && !dragCoefficient.Value.Equals("-"))
                currentModifier.DragCoefficient = float.Parse(dragCoefficient.Value.Replace(".",","));

            if (density.Value != null && !density.Value.Equals("") && !density.Value.Equals("-"))
                currentModifier.Density = float.Parse(density.Value.Replace(".", ","));

            _modifiers[key] = currentModifier;
        }
    }
}
