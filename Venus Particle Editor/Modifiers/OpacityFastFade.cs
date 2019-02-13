using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using VenusParticleEngine.Core.Modifiers;
using System.Collections.Generic;

namespace VenusParticleEditor.Modifiers
{
    class OpacityFastFade
    {
        private CheckBox checkboxActive;
        private string key = "OpacityFastFade";
        private Panel panelMaster;
        private Panel panelInside;
        private Vector2 masterSize = new Vector2(-1, 100);
        private Vector2 masterSizeMinimized = new Vector2(-1, 100);
        private Dictionary<string, IModifier> _modifiers;

        public Panel Panel
        {
            get { return panelMaster; }
        }

        public OpacityFastFade()
        {
            panelMaster = new Panel(masterSize, PanelSkin.None, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            panelInside = new Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 100), PanelSkin.Simple, anchor: Anchor.AutoInline, offset: new Vector2(0, 0));
            checkboxActive = new CheckBox("Active", isChecked: false)
            {
                OnClick = (Entity btn) => { OnClickCheckboxActive(); }
            };

            panelMaster.AddChild(new RichParagraph("{{GOLD}} Opacity Fast Fade"));
            panelMaster.AddChild(checkboxActive);
            panelMaster.AddChild(panelInside);

            panelInside.Visible = false;
            panelMaster.Size = masterSizeMinimized;
        }

        public void Refresh(Dictionary<string, IModifier> modifiers)
        {
            _modifiers = modifiers;
            if (_modifiers != null && _modifiers.ContainsKey(key))
            {
            }
        }

        private void OnClickCheckboxActive()
        {
            if (checkboxActive.Checked)
            {
                _modifiers.Add(key, new VenusParticleEngine.Core.Modifiers.OpacityFastFadeModifier());
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
                // panelInside.Visible = true;
                panelMaster.Size = masterSize;
                checkboxActive.Checked = true;
            }
            else
            {
                //   panelInside.Visible = false;
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
            OpacityFastFadeModifier currentModifier = (OpacityFastFadeModifier)modifier;

            _modifiers[key] = currentModifier;
        }
    }
}
