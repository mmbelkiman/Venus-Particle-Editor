#region Using Statements
using System;
using GeonBit.UI;
using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using VenusParticleEngine.Core;
using VenusParticleEditor.Modifiers;
using VenusParticleEngine.Core.Profiles;
using static VenusParticleEngine.Core.Profiles.Profile;
#endregion

/***
 * Venus Particle Editor
 * 1.0.0.2 feb/2019
 * Marcelo Belkiman @mmbelkiman
 * Icon made by Smashicons from www.flaticon.com
 * */
namespace VenusParticleEditor
{
    public class MainScreen : Game
    {
        #region ============= Enum
        private enum EnumEmitterProfiles { BoxProfile, BoxFillProfile, BoxUniformProfile, CircleProfile, LineProfile, PointProfile, RingProfile, SprayProfile };
        private enum EnumEmitterCircleRadiation { In, Out, None };
        private enum EnumEmitterAxis { Up, Down, Left, Right };
        private enum EnumEmitterBlendMode { Add, Alpha, Subtract };
        private enum EnumExecutionStrategy { Parallel, Serial };
        #endregion

        #region ============= Fields final
        private readonly string EMITTER_PRINCIPAL_NAME = "principal";
        private readonly float FONT_SCALE = 1f;
        private readonly int TOP_PANEL_HEIGHT = 65;
        #endregion

        #region ============= Fields
        private GraphicsDeviceManager _graphics;
        private ParticleEffect _particleEffect;
        private KeyboardHelper _keyboardHelper;
        private LoggerHelper _logger;
        private SpriteBatch _spriteBatch;
        private Color _currentClearColor = Color.CornflowerBlue;

        private string _currentEmitter;
        private bool _queueActionExport = false;
        private bool _queueActionLoad = false;
        private bool _moveCursorMode = false;
        private float _zoom = 1.0f;

        private Modifiers.CircleContainer _modifierCircleContainer;
        private Modifiers.ColourInterpolator2 _modifierColourInterpolator2;
        private Modifiers.Drag _modifierDrag;
        private Modifiers.HueInterpolator2 _modifierHueInterpolator2;
        private Modifiers.LinearGravity _modifierLinearGravity;
        private Modifiers.OpacityFastFade _modifierOpacityFastFade;
        private Modifiers.OpacityInterpolator2 _modifierOpacityInterpolator2;
        private Modifiers.OpacityInterpolator3 _modifierOpacityInterpolator3;
        private Modifiers.RectContainer _modifierRectContainer;
        private Modifiers.RectLoopContainer _modifierRectLoopContainer;
        private Modifiers.Rotation _modifierRotation;
        private Modifiers.ScaleInterpolator2 _modifierScaleInterpolator2;
        private Modifiers.VelocityColour _modifierVelocityColour;
        private Modifiers.VelocityColourInfinite _modifierVelocityColourInfinite;
        private Modifiers.VelocityHue _modifierVelocityHue;
        private Modifiers.Vortex _modifierVortex;

        private GeonBit.UI.Entities.Panel _texturePanelImage;
        private GeonBit.UI.Entities.Panel _panelEditor;
        private GeonBit.UI.Entities.RadioButton _textureRadioSquare;
        private GeonBit.UI.Entities.RadioButton _textureRadioImage;
        private GeonBit.UI.Entities.Button _buttonRemoveEmitter;
        private Paragraph _parametersParagraphColorH;
        private Paragraph _parametersParagraphColorS;
        private Paragraph _parametersParagraphColorL;
        private Paragraph _parametersParagraphOpacity;
        private RichParagraph _paragraphZoomText;
        private Paragraph _paragraphImageName;
        private Slider _parametersSliderColorH;
        private Slider _parametersSliderColorS;
        private Slider _parametersSliderColorL;
        private Slider _parametersSliderOpacity;
        private DropDown _dropdownEmitter;
        private DropDown _dropdownProfile;
        private DropDown _dropdownProfileCircleRadiation;
        private DropDown _dropdownProfileAxis;
        private DropDown _dropdownBlendMode;
        private DropDown _dropdownExecutionStrategy;
        private TextInput _inputCapacity;
        private TextInput _inputTimeMilleseconds;
        private TextInput _inputProfileWidth;
        private TextInput _inputProfileHeight;
        private TextInput _inputProfileRadius;
        private TextInput _inputProfileLength;
        private TextInput _inputProfileSpread;
        private TextInput _inputScale;
        private TextInput _inputQuantity;
        private TextInput _inputReclaimFrequency;
        private TextInput _inputOffsetX;
        private TextInput _inputOffsetY;
        private TextInput _inputSpeedMin;
        private TextInput _inputSpeedMax;
        private TextInput _inputRotationMin;
        private TextInput _inputRotationMax;
        private TextInput _inputMassMin;
        private TextInput _inputMassMax;
        #endregion

        #region ============= Constructor
        public MainScreen()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GlobalFields.APP_WIDTH,
                PreferredBackBufferHeight = GlobalFields.APP_HEIGHT,
                // PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = true,
                IsFullScreen = false,
                HardwareModeSwitch = false,
                SupportedOrientations = DisplayOrientation.LandscapeRight,
            };
            Content.RootDirectory = "Content";
        }
        #endregion

        #region ============= Superclass methods
        protected override void Initialize()
        {
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;
            UserInterface.Active.IncludeCursorInRenderTarget = false;

            _particleEffect = new ParticleEffect();
            _keyboardHelper = new KeyboardHelper();
            _logger = new LoggerHelper();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitializeUI();
            AddEmitter(EMITTER_PRINCIPAL_NAME);

            base.Initialize();

            _logger.Info("Venus Particle Editor Ver: " + GlobalFields.SOFTWARE_VERSION);
            _logger.Warning("by: marcelo belkiman @mmbelkiman");
            _logger.Info("Geon Bit UI ver: " + GlobalFields.GEONBITUI_VERION);
            _logger.Info("Newtonsoft.Json ver: " + Assembly.LoadFrom("Newtonsoft.Json.dll").GetName().Version);
            _logger.Info("Monogame ver: " + Assembly.LoadFrom("Monogame.Framework.dll").GetName().Version);
            _logger.Info("Venus Particle engine ver: " + Assembly.LoadFrom("VenusParticleEngine.dll").GetName().Version);
            _logger.Info(
                "F6 zoom out"
                + " / F7 reset zoom"
                + " / F8 zoom in"
               + " / F9 move particle"
               + " / F10 change background"
               + "/ F11 full screen mode"
               + "/ F12 show/hide log");
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {
                UserInterface.Active.Draw(_spriteBatch);
                GraphicsDevice.Clear(_currentClearColor);
                UserInterface.Active.DrawMainRenderTarget(_spriteBatch);
            }
            catch (Exception e)
            {
                GraphicsDevice.Clear(_currentClearColor);
                UserInterface.Active.Dispose();
                _logger.Error(e.Message);
            }

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: Matrix.CreateScale(_zoom));
            _spriteBatch.Draw(_particleEffect);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_queueActionExport)
                Export();

            if (_queueActionLoad)
                Load();

            if (_moveCursorMode)
            { _particleEffect.Trigger(new Vector(Mouse.GetState().X/_zoom, Mouse.GetState().Y/_zoom)); }
            else
            { _particleEffect.Trigger(new Vector(150/_zoom, 400/_zoom)); }

            UserInterface.Active.Update(gameTime);

            _keyboardHelper.Update(gameTime);

            if (!_particleEffect.Emitters.ContainsKey(_currentEmitter)) return;

            _modifierCircleContainer.Update();
            _modifierColourInterpolator2.Update();
            _modifierDrag.Update();
            _modifierHueInterpolator2.Update();
            _modifierLinearGravity.Update();
            _modifierOpacityFastFade.Update();
            _modifierOpacityInterpolator2.Update();
            _modifierOpacityInterpolator3.Update();
            _modifierRectContainer.Update();
            _modifierRectLoopContainer.Update();
            _modifierRotation.Update();
            _modifierScaleInterpolator2.Update();
            _modifierVelocityColour.Update();
            _modifierVelocityColourInfinite.Update();
            _modifierVelocityHue.Update();
            _modifierVortex.Update();

            if (_textureRadioImage.Checked)
            { _texturePanelImage.Visible = true; }
            else
            { _texturePanelImage.Visible = false; }

            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            UpdateCapacity();
            UpdateTimeMilleseconds();
            UpdateProfile();
            UpdateBlendMode();
            UpdateReclaimFrequency();
            UpdateExecutionStrategy();
            UpdateOffset();
            UpdateColor();
            UpdateOpacity();
            UpdateQuantity();
            UpdateSpeed();
            UpdateScale();
            UpdateRotation();
            UpdateMass();

            UpdateKeyboard();

            base.Update(gameTime);
        }
        #endregion

        private void CreateTopMenu()
        {
            // create top panel
            GeonBit.UI.Entities.Panel topPanel = new GeonBit.UI.Entities.Panel(new Vector2(0, TOP_PANEL_HEIGHT + 2), PanelSkin.Simple, Anchor.TopCenter);
            topPanel.Padding = Vector2.Zero;
            UserInterface.Active.AddEntity(topPanel);

            // add export button
            GeonBit.UI.Entities.Button buttonExport = new GeonBit.UI.Entities.Button("Export", ButtonSkin.Default, Anchor.AutoInline, new Vector2(170, TOP_PANEL_HEIGHT))
            {
                OnClick = (Entity btn) => { this.Export(); }
            };
            topPanel.AddChild(buttonExport);

            // add Load button
            GeonBit.UI.Entities.Button buttonLoad = new GeonBit.UI.Entities.Button("Load", ButtonSkin.Default, Anchor.AutoInline, new Vector2(170, TOP_PANEL_HEIGHT))
            {
                OnClick = (Entity btn) => { this.Load(); }
            };
            topPanel.AddChild(buttonLoad);

            // add Load button
            GeonBit.UI.Entities.Button buttonMove = new GeonBit.UI.Entities.Button("Move", ButtonSkin.Default, Anchor.AutoInline, new Vector2(170, TOP_PANEL_HEIGHT))
            {
                OnClick = (Entity btn) =>
                {
                    _moveCursorMode = !_moveCursorMode;
                }
            };
            topPanel.AddChild(buttonMove);

            // add title and text
            var welcomeText = new RichParagraph("{{ORANGE}}     Venus {{DEFAULT}}Particle Editor", Anchor.TopLeft, new Vector2(180, TOP_PANEL_HEIGHT), offset: new Vector2(50, 90));
            UserInterface.Active.AddEntity(welcomeText);

            _paragraphZoomText = new RichParagraph("Zoom ", Anchor.TopLeft, new Vector2(180, TOP_PANEL_HEIGHT), offset: new Vector2(50, 150), scale : 0.8f);
            UserInterface.Active.AddEntity(_paragraphZoomText);

            //Dropdown Emitter
            _dropdownEmitter = new DropDown(new Vector2(290, -1), anchor: Anchor.TopRight, offset: new Vector2(170, 0))
            {
                DefaultText = "Click to change",
                OnValueChange = (entity) =>
                {
                    //Add new Emitter
                    if (_dropdownEmitter.SelectedIndex == _dropdownEmitter.Count - 1)
                    {
                        var textInput = new TextInput(false)
                        {
                            PlaceholderText = "Name"
                        };
                        GeonBit.UI.Utils.MessageBox.ShowMsgBox(
                            "Reffer", "Please insert name to new entitiy",
                            new GeonBit.UI.Utils.MessageBox.MsgBoxOption[] {
                                        new GeonBit.UI.Utils.MessageBox.MsgBoxOption("ADD", () =>
                                        {
                                            if (string.IsNullOrEmpty(textInput.Value))
                                            {
                                                textInput.Value = (_dropdownEmitter.Count + 1).ToString();
                                            }

                                            AddEmitter(textInput.Value);
                                            _buttonRemoveEmitter.Enabled = true;

                                            return true;
                                        }),
                            }, new Entity[] { textInput });
                    }
                    else
                    {
                        if (_dropdownEmitter.SelectedValue != null)
                            _currentEmitter = _dropdownEmitter.SelectedValue;
                        RefreshEmitter();
                    }
                }
            };

            _dropdownEmitter.AddItem("Add new");
            UserInterface.Active.AddEntity(_dropdownEmitter);

            _buttonRemoveEmitter = new GeonBit.UI.Entities.Button("Remove", ButtonSkin.Default, Anchor.TopRight, new Vector2(170, TOP_PANEL_HEIGHT), offset: new Vector2(0, 0))
            {
                Enabled = false,
                OnClick = (Entity btn) =>
                {
                    if (_dropdownEmitter.SelectedIndex >= 0)
                    {
                        _logger.Info("remove emitter => " + _dropdownEmitter.SelectedValue + " at pos " + _dropdownEmitter.SelectedIndex);
                        _particleEffect.Emitters.Remove(_dropdownEmitter.SelectedValue);

                        _dropdownEmitter.RemoveItem(_dropdownEmitter.SelectedIndex);
                        _dropdownEmitter.SelectedIndex = -1;

                        if (_dropdownEmitter.Count == 2)
                        {
                            _buttonRemoveEmitter.Enabled = false;
                        }

                    }
                }
            };
            UserInterface.Active.AddEntity(_buttonRemoveEmitter);
        }

        private void InitializeUI()
        {
            CreateTopMenu();

            // Add console panel
            var Name = new RichParagraph("by: Marcelo Belkiman @mmbelkiman", Anchor.BottomLeft);
            UserInterface.Active.AddEntity(Name);
            UserInterface.Active.AddEntity(_logger.PanelConsole);

            // add exit button
            GeonBit.UI.Entities.Button exitBtn = new GeonBit.UI.Entities.Button("Exit", anchor: Anchor.BottomRight, size: new Vector2(200, -1))
            {
                OnClick = (Entity entity) => { Exit(); }
            };
            UserInterface.Active.AddEntity(exitBtn);

            // Panel:
            {
                _panelEditor = new GeonBit.UI.Entities.Panel(GlobalFields.PANEL_EDITOR_SIZE, PanelSkin.Fancy, Anchor.CenterRight, offset: new Vector2(0, -40));
                UserInterface.Active.AddEntity(_panelEditor);

                // create panel tabs
                PanelTabs tabs = new PanelTabs
                {
                    BackgroundSkin = PanelSkin.Default
                };
                _panelEditor.AddChild(tabs);

                // add panel
                {
                    _textureRadioImage = new GeonBit.UI.Entities.RadioButton("Image", Anchor.AutoInline, new Vector2(0.5f, -1f))
                    {
                        OnClick = (Entity btn) => { this.LoadImage(); }
                    };
                }
                {
                    _textureRadioSquare = new GeonBit.UI.Entities.RadioButton("Square", Anchor.AutoInline, new Vector2(0.5f, -1f), isChecked: true)
                    {
                        OnClick = (Entity btn) => { this.EmitterToSquare(); }
                    };
                }

                // add first panel
                {
                    TabData tab = tabs.AddTab("Emitter");

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Type"));

                        tab.panel.AddChild(_textureRadioSquare);
                        tab.panel.AddChild(_textureRadioImage);
                    }

                    {
                        _texturePanelImage = new GeonBit.UI.Entities.Panel(new Vector2(GlobalFields.PANEL_EDITOR_INSIDE_SIZE.X, 100), anchor: Anchor.AutoInline, skin: PanelSkin.Simple, offset: new Vector2(0, 0));
                        _paragraphImageName = new Paragraph("name:", Anchor.AutoInline, scale: FONT_SCALE);
                        _texturePanelImage.AddChild(_paragraphImageName);
                        tab.panel.AddChild(_texturePanelImage);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Capacity", Anchor.AutoInline, new Vector2(0.3f, -1), scale: FONT_SCALE));
                        _inputCapacity = new TextInput(false, new Vector2(0.7f, -1f), anchor: Anchor.AutoInline);
                        _inputCapacity.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputCapacity.Value = "0";
                        _inputCapacity.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputCapacity);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Time Milliseconds", Anchor.AutoInline, new Vector2(0.3f, -1), scale: FONT_SCALE));
                        _inputTimeMilleseconds = new TextInput(false, new Vector2(0.7f, -1f), anchor: Anchor.AutoInline);
                        _inputTimeMilleseconds.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputTimeMilleseconds.Value = "1000";
                        _inputTimeMilleseconds.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputTimeMilleseconds);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Profile", Anchor.AutoInline, new Vector2(0.3f, -1), scale: FONT_SCALE));

                        _dropdownProfile = new DropDown(new Vector2(0.7f, -1), anchor: Anchor.AutoInline)
                        {
                            DefaultText = "Click to change",
                            OnValueChange = (entity) =>
                            {
                                UpdateProfile();
                                RefreshEmitterProfile();
                            }
                        };
                        _dropdownProfile.AddItem(EnumEmitterProfiles.BoxProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.BoxFillProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.BoxUniformProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.CircleProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.LineProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.PointProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.RingProfile.ToString());
                        _dropdownProfile.AddItem(EnumEmitterProfiles.SprayProfile.ToString());
                        tab.panel.AddChild(_dropdownProfile);

                        tab.panel.AddChild(new Paragraph("Width", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputProfileWidth = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputProfileWidth.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputProfileWidth.Value = "1";
                        _inputProfileWidth.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputProfileWidth);

                        tab.panel.AddChild(new Paragraph("Height", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputProfileHeight = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputProfileHeight.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputProfileHeight.Value = "1";
                        _inputProfileHeight.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputProfileHeight);

                        tab.panel.AddChild(new Paragraph("radius", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _inputProfileRadius = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
                        _inputProfileRadius.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputProfileRadius.Value = "1";
                        _inputProfileRadius.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputProfileRadius);

                        tab.panel.AddChild(new Paragraph("CircleRadiation", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _dropdownProfileCircleRadiation = new DropDown(new Vector2(0.5f, -1), anchor: Anchor.AutoInline)
                        {
                            DefaultText = "Click to change",
                            OnValueChange = (entity) =>
                            {
                                UpdateProfile();
                                RefreshEmitterProfile();
                            }
                        };
                        _dropdownProfileCircleRadiation.AddItem(EnumEmitterCircleRadiation.In.ToString());
                        _dropdownProfileCircleRadiation.AddItem(EnumEmitterCircleRadiation.Out.ToString());
                        _dropdownProfileCircleRadiation.AddItem(EnumEmitterCircleRadiation.None.ToString());
                        tab.panel.AddChild(_dropdownProfileCircleRadiation);

                        tab.panel.AddChild(new Paragraph("Axis", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _dropdownProfileAxis = new DropDown(new Vector2(0.5f, -1), anchor: Anchor.AutoInline)
                        {
                            DefaultText = "Click to change",
                            OnValueChange = (entity) =>
                            {
                                UpdateProfile();
                                RefreshEmitterProfile();
                            }
                        };
                        _dropdownProfileAxis.AddItem(EnumEmitterAxis.Up.ToString());
                        _dropdownProfileAxis.AddItem(EnumEmitterAxis.Down.ToString());
                        _dropdownProfileAxis.AddItem(EnumEmitterAxis.Left.ToString());
                        _dropdownProfileAxis.AddItem(EnumEmitterAxis.Right.ToString());
                        tab.panel.AddChild(_dropdownProfileAxis);

                        tab.panel.AddChild(new Paragraph("Length", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _inputProfileLength = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
                        _inputProfileLength.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputProfileLength.Value = "1";
                        _inputProfileLength.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputProfileLength);

                        tab.panel.AddChild(new Paragraph("Spread", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _inputProfileSpread = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
                        _inputProfileSpread.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputProfileSpread.Value = "1";
                        _inputProfileSpread.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputProfileSpread);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Blend Mode", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _dropdownBlendMode = new DropDown(new Vector2(0.5f, -1), anchor: Anchor.AutoInline)
                        {
                            DefaultText = "Click to change",
                            OnValueChange = (entity) =>
                            {
                                UpdateBlendMode();
                            }
                        };
                        _dropdownBlendMode.AddItem(EnumEmitterBlendMode.Add.ToString());
                        _dropdownBlendMode.AddItem(EnumEmitterBlendMode.Alpha.ToString());
                        _dropdownBlendMode.AddItem(EnumEmitterBlendMode.Subtract.ToString());
                        tab.panel.AddChild(_dropdownBlendMode);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Reclaim Frequency", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _inputReclaimFrequency = new TextInput(false, new Vector2(0.5f, -1f), anchor: Anchor.AutoInline);
                        _inputReclaimFrequency.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputReclaimFrequency.Value = "1";
                        _inputReclaimFrequency.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputReclaimFrequency);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Execution Strategy", Anchor.AutoInline, new Vector2(0.5f, -1), scale: FONT_SCALE));
                        _dropdownExecutionStrategy = new DropDown(new Vector2(0.5f, -1), anchor: Anchor.AutoInline)
                        {
                            DefaultText = "Click to change",
                            OnValueChange = (entity) =>
                            {
                                UpdateExecutionStrategy();
                            }
                        };
                        _dropdownExecutionStrategy.AddItem(EnumExecutionStrategy.Parallel.ToString());
                        _dropdownExecutionStrategy.AddItem(EnumExecutionStrategy.Serial.ToString());
                        tab.panel.AddChild(_dropdownExecutionStrategy);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Offset"));

                        tab.panel.AddChild(new Paragraph("X", Anchor.AutoInline, new Vector2(0.1f, -1), scale: FONT_SCALE));
                        _inputOffsetX = new TextInput(false, new Vector2(0.4f, -1f), anchor: Anchor.AutoInline);
                        _inputOffsetX.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
                        _inputOffsetX.Value = "1";
                        _inputOffsetX.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputOffsetX);

                        tab.panel.AddChild(new Paragraph("Y", Anchor.AutoInline, new Vector2(0.1f, -1), scale: FONT_SCALE));
                        _inputOffsetY = new TextInput(false, new Vector2(0.4f, -1f), anchor: Anchor.AutoInline);
                        _inputOffsetY.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
                        _inputOffsetY.Value = "1";
                        _inputOffsetY.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputOffsetY);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Color HSL"));

                        _parametersSliderColorH = new Slider(0, 360, SliderSkin.Default);
                        _parametersParagraphColorH = new Paragraph("HUE", scale: FONT_SCALE);
                        tab.panel.AddChild(_parametersParagraphColorH);
                        tab.panel.AddChild(_parametersSliderColorH);

                        _parametersSliderColorS = new Slider(0, 100, SliderSkin.Default);
                        _parametersParagraphColorS = new Paragraph("Saturation", scale: FONT_SCALE);
                        tab.panel.AddChild(_parametersParagraphColorS);
                        tab.panel.AddChild(_parametersSliderColorS);

                        _parametersSliderColorL = new Slider(0, 100, SliderSkin.Default);
                        _parametersParagraphColorL = new Paragraph("Lightness", scale: FONT_SCALE);
                        tab.panel.AddChild(_parametersParagraphColorL);
                        tab.panel.AddChild(_parametersSliderColorL);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _parametersSliderOpacity = new Slider(0, 100, SliderSkin.Default);
                        _parametersParagraphOpacity = new Paragraph("Opacity", scale: FONT_SCALE);
                        tab.panel.AddChild(_parametersParagraphOpacity);
                        tab.panel.AddChild(_parametersSliderOpacity);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Quantity", Anchor.AutoInline, new Vector2(0.3f, -1), scale: FONT_SCALE));
                        _inputQuantity = new TextInput(false, new Vector2(0.7f, -1f), anchor: Anchor.AutoInline);
                        _inputQuantity.Validators.Add(new TextValidatorNumbersOnly(false, 0));
                        _inputQuantity.Value = "1";
                        _inputQuantity.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputQuantity);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Speed"));

                        tab.panel.AddChild(new Paragraph("Min", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputSpeedMin = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputSpeedMin.Validators.Add(new TextValidatorNumbersOnly(true, 0));
                        _inputSpeedMin.Value = "1";
                        _inputSpeedMin.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputSpeedMin);

                        tab.panel.AddChild(new Paragraph("Max", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputSpeedMax = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputSpeedMax.Validators.Add(new TextValidatorNumbersOnly(true, 0));
                        _inputSpeedMax.Value = "1";
                        _inputSpeedMax.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputSpeedMax);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new Paragraph("Scale", Anchor.AutoInline, new Vector2(0.3f, -1), scale: FONT_SCALE));
                        _inputScale = new TextInput(false, new Vector2(0.7f, -1f), anchor: Anchor.AutoInline);
                        _inputScale.Validators.Add(new TextValidatorNumbersOnly(true, 0));
                        _inputScale.Value = "1";
                        _inputScale.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputScale);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Rotation"));

                        tab.panel.AddChild(new Paragraph("Min", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputRotationMin = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputRotationMin.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
                        _inputRotationMin.Value = "1";
                        _inputRotationMin.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputRotationMin);

                        tab.panel.AddChild(new Paragraph("Max", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputRotationMax = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputRotationMax.Validators.Add(new TextValidatorNumbersOnly(allowDecimal: true));
                        _inputRotationMax.Value = "1";
                        _inputRotationMax.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputRotationMax);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        tab.panel.AddChild(new RichParagraph("{{GOLD}} Mass"));

                        tab.panel.AddChild(new Paragraph("Min", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputMassMin = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputMassMin.Validators.Add(new TextValidatorNumbersOnly(true));
                        _inputMassMin.Value = "1";
                        _inputMassMin.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputMassMin);

                        tab.panel.AddChild(new Paragraph("Max", Anchor.AutoInline, new Vector2(0.2f, -1), scale: FONT_SCALE));
                        _inputMassMax = new TextInput(false, new Vector2(0.3f, -1f), anchor: Anchor.AutoInline);
                        _inputMassMax.Validators.Add(new TextValidatorNumbersOnly(true));
                        _inputMassMax.Value = "1";
                        _inputMassMax.ValueWhenEmpty = "1";
                        tab.panel.AddChild(_inputMassMax);
                    }

                    tab.panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
                    tab.panel.Scrollbar.AdjustMaxAutomatically = true;
                }

                // add Modifiers panel
                {
                    TabData tab = tabs.AddTab("Modifiers");

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierCircleContainer = new Modifiers.CircleContainer();
                        tab.panel.AddChild(_modifierCircleContainer.Panel);
                    }


                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierColourInterpolator2 = new Modifiers.ColourInterpolator2();
                        tab.panel.AddChild(_modifierColourInterpolator2.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierDrag = new Drag();
                        tab.panel.AddChild(_modifierDrag.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierHueInterpolator2 = new Modifiers.HueInterpolator2();
                        tab.panel.AddChild(_modifierHueInterpolator2.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierLinearGravity = new Modifiers.LinearGravity();
                        tab.panel.AddChild(_modifierLinearGravity.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierOpacityFastFade = new Modifiers.OpacityFastFade();
                        tab.panel.AddChild(_modifierOpacityFastFade.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierOpacityInterpolator2 = new Modifiers.OpacityInterpolator2();
                        tab.panel.AddChild(_modifierOpacityInterpolator2.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierOpacityInterpolator3 = new Modifiers.OpacityInterpolator3();
                        tab.panel.AddChild(_modifierOpacityInterpolator3.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierRectContainer = new Modifiers.RectContainer();
                        tab.panel.AddChild(_modifierRectContainer.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierRectLoopContainer = new Modifiers.RectLoopContainer();
                        tab.panel.AddChild(_modifierRectLoopContainer.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierRotation = new Modifiers.Rotation();
                        tab.panel.AddChild(_modifierRotation.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierScaleInterpolator2 = new Modifiers.ScaleInterpolator2();
                        tab.panel.AddChild(_modifierScaleInterpolator2.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierVelocityColour = new Modifiers.VelocityColour();
                        tab.panel.AddChild(_modifierVelocityColour.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierVelocityColourInfinite = new Modifiers.VelocityColourInfinite();
                        tab.panel.AddChild(_modifierVelocityColourInfinite.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierVelocityHue = new Modifiers.VelocityHue();
                        tab.panel.AddChild(_modifierVelocityHue.Panel);
                    }

                    tab.panel.AddChild(new HorizontalLine());
                    {
                        _modifierVortex = new Modifiers.Vortex();
                        tab.panel.AddChild(_modifierVortex.Panel);
                    }

                    tab.panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
                    tab.panel.Scrollbar.AdjustMaxAutomatically = true;
                }
            }
        }

        private void Export()
        {
            if (_graphics.IsFullScreen)
            {
                ToggleFullscreen();
                _queueActionExport = true;
                return;
            }

            _queueActionExport = false;
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title = "Save Particle",
                Filter = "Particle (*.ptc;)|*.ptc;"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    JsonHelper.WriteToJsonFile(fileDialog.FileName, _particleEffect);
                    _logger.Info("EXPORT: " + fileDialog.FileName);
                }
                catch (Exception e)
                {
                    _logger.Error("EXPORT: " + e.Message);
                }

                foreach (KeyValuePair<string, Emitter> keyValue in _particleEffect.Emitters)
                {
                    try
                    {
                        if (keyValue.Value.Texture.Name != null && !keyValue.Value.Texture.Name.Equals(""))
                        {

                            if (System.IO.File.Exists(Path.GetDirectoryName(fileDialog.FileName) + "\\" + Path.GetFileName(keyValue.Value.Texture.Name)))
                            {
                                _logger.Warning("EXPORT IGNORE IMG " + Path.GetDirectoryName(fileDialog.FileName) + "\\" + Path.GetFileName(keyValue.Value.Texture.Name));
                            }
                            else
                            {
                                File.Copy(
                                    keyValue.Value.Texture.Name,
                                    Path.GetDirectoryName(fileDialog.FileName) + "\\" + Path.GetFileName(keyValue.Value.Texture.Name),
                                    true);

                                _logger.Info("EXPORT IMG " + Path.GetDirectoryName(fileDialog.FileName) + "\\" + Path.GetFileName(keyValue.Value.Texture.Name));
                            }
                        }
                    }
                    catch (DirectoryNotFoundException dirNotFound)
                    {
                        _logger.Error(dirNotFound.Message);
                    }
                    catch (IOException ioEx)
                    {
                        _logger.Error(ioEx.Message);
                    }
                }
            }
        }

        private void EmitterToSquare()
        {
            Texture2D Texture = new Texture2D(GraphicsDevice, 1, 1);

            _particleEffect.Emitters[_currentEmitter].Texture = Texture;
            _particleEffect.Emitters[_currentEmitter].Texture.SetData(new[] { Color.White });
        }

        private void LoadImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Title = "Load Image";
            fileDialog.Filter = "PNG (*.png;)|*.png;" +
                                "|jpg (*.jpg;)|*.jpg;" +
                                "|XNB (*.xnb;)|*.xnb;" +
                                "|All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (fileDialog.FileName.Contains(".xnb"))
                {
                    LoadImageXNB(fileDialog.FileName);
                }
                else
                {
                    LoadImageNormal(fileDialog.FileName);
                }

                if (_particleEffect.Emitters[_currentEmitter].Texture.Width > _particleEffect.Emitters[_currentEmitter].Texture.Height)
                {
                    _inputScale.Value = _particleEffect.Emitters[_currentEmitter].Texture.Width.ToString();
                }
                else
                {
                    _inputScale.Value = _particleEffect.Emitters[_currentEmitter].Texture.Height.ToString();
                }
                _paragraphImageName.Text = "name:" + Path.GetFileName(fileDialog.FileName);
            }
            else
            {
                _textureRadioImage.Checked = false;
                _textureRadioSquare.Checked = true;
            }
        }

        private void LoadImageNormal(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            Texture2D spriteAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
            spriteAtlas.Name = fileName;
            fileStream.Dispose();
            _particleEffect.Emitters[_currentEmitter].Texture = spriteAtlas;
            _particleEffect.Emitters[_currentEmitter].Texture.Name = fileName;
            _parametersSliderColorL.Value = 100;
        }

        private void LoadImageXNB(string fileName)
        {
            _particleEffect.Emitters[_currentEmitter].Texture = Content.Load<Texture2D>(fileName.Replace(".xnb", ""));
        }

        private void Load()
        {
            if (_graphics.IsFullScreen)
            {
                ToggleFullscreen();
                _queueActionLoad = true;
                return;
            }

            _queueActionLoad = false;

            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Load particle",
                Filter = "Particle (*.ptc;)|*.ptc;" +
                                     "|All Files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //Remove all items from dropdown
                _dropdownEmitter.SelectedIndex = -1;
                int countDropdown = _dropdownEmitter.Count;
                for (int i = 0; i < countDropdown; i++)
                {
                    _dropdownEmitter.RemoveItem(0);
                }

                ParticleEffect pe = ParticleEffect.ReadFromJsonFile(fileDialog.FileName, GraphicsDevice, Content);

                //Add new items at dropdown
                foreach (var item in pe.Emitters)
                {
                    _dropdownEmitter.AddItem(item.Key);
                }
                _dropdownEmitter.AddItem("Add new");
                _dropdownEmitter.SelectedIndex = 0;
                _currentEmitter = _dropdownEmitter.SelectedValue;

                if (_dropdownEmitter.Count > 2)
                {
                    _buttonRemoveEmitter.Enabled = true;
                }
                else
                {
                    _buttonRemoveEmitter.Enabled = false;
                }

                foreach (var item in pe.Emitters)
                {
                    if (string.IsNullOrEmpty(item.Value.TexturePath))
                    {
                        item.Value.Texture = new Texture2D(GraphicsDevice, 1, 1);
                        item.Value.Texture.SetData(new[] { Color.White });
                    }
                }

                _particleEffect = pe;

                _inputProfileHeight.Value = _particleEffect.Emitters[_currentEmitter].Profile.Height.ToString();
                _inputProfileWidth.Value = _particleEffect.Emitters[_currentEmitter].Profile.Width.ToString();
                _inputProfileLength.Value = _particleEffect.Emitters[_currentEmitter].Profile.Length.ToString();
                _inputProfileSpread.Value = _particleEffect.Emitters[_currentEmitter].Profile.Spread.ToString();
                _inputProfileRadius.Value = _particleEffect.Emitters[_currentEmitter].Profile.Radius.ToString();

                RefreshEmitter();

                if (_particleEffect.Emitters[_currentEmitter].TexturePath.Equals(""))
                {
                    _textureRadioSquare.Checked = true;
                }
                else
                {
                    _textureRadioImage.Checked = true;
                    _paragraphImageName.Text = "name:" + Path.GetFileName(_particleEffect.Emitters[_currentEmitter].TexturePath);
                }

                _logger.Info("LOAD " + Path.GetDirectoryName(fileDialog.FileName));
            }
        }

        private void AddEmitter(string Name)
        {
            _particleEffect.Emitters.Add(Name,
                new Emitter(100, TimeSpan.FromMilliseconds(1000f), Profile.Point())
                {
                    Texture = new Texture2D(GraphicsDevice, 1, 1),
                    BlendMode = BlendMode.Alpha,
                    ReclaimFrequency = 1f,
                    ForceLoop = true,
                    ModifierExecutionStrategy = ModifierExecutionStrategy.Serial(),
                    Offset = Vector.Zero,
                    Parameters = new ReleaseParameters
                    {
                        Colour = new Colour(0, .5f, .5f),
                        Opacity = 1f,
                        Quantity = 1,
                        Speed = 1.1f,
                        Scale = 10f,
                        Rotation = 0f,
                        Mass = 1.1f
                    },
                    SpriteEffects = SpriteEffects.None,
                    LayerDepth = 0
                });

            _particleEffect.Emitters[Name].Texture.SetData(new[] { Color.White });

            _dropdownEmitter.RemoveItem("Add new");
            _dropdownEmitter.AddItem(Name);
            _dropdownEmitter.AddItem("Add new");
            _dropdownEmitter.SelectedIndex = _dropdownEmitter.Count - 2;
            _currentEmitter = _dropdownEmitter.SelectedValue;

            _textureRadioSquare.Checked = true;
            _textureRadioImage.Checked = false;

            if (!Name.Equals(EMITTER_PRINCIPAL_NAME))
                _logger.Info("add emitter => " + Name + " at pos " + (_particleEffect.Emitters.Count - 1));
        }

        private void UpdateKeyboard()
        {
            _paragraphZoomText.Text = "zoom " + _zoom;

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                _zoom -= 0.1f;
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F7))
            {
                _zoom = 1.0f;
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F8))
            {
                _zoom += 0.1f;
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F9))
            {
                _moveCursorMode = !_moveCursorMode;
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F10))
            {
                if (_currentClearColor == Color.White)
                {
                    _currentClearColor = Color.CornflowerBlue;
                    return;
                }
                if (_currentClearColor == Color.Black)
                    _currentClearColor = Color.White;
                if (_currentClearColor == Color.CornflowerBlue)
                    _currentClearColor = Color.Black;
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F11))
            {
                ToggleFullscreen();
            }

            if (_keyboardHelper.IsNewKeyRelease(Microsoft.Xna.Framework.Input.Keys.F12))
            {
                _logger.Active = !_logger.Active;
            }
        }

        private void ToggleFullscreen()
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            if (_graphics.IsFullScreen)
            {
                _panelEditor.Size = GlobalFields.PANEL_EDITOR_SIZE_FULLSCREEN;
            }
            else
            {
                _panelEditor.Size = GlobalFields.PANEL_EDITOR_SIZE;
            }

            _graphics.ApplyChanges();
        }

        private void UpdateBlendMode()
        {
            if (_particleEffect.Emitters[_currentEmitter].BlendMode.ToString() != _dropdownBlendMode.SelectedValue)
            {
                switch (_dropdownBlendMode.SelectedValue)
                {
                    case nameof(EnumEmitterBlendMode.Add):
                        _particleEffect.Emitters[_currentEmitter].BlendMode = BlendMode.Add;
                        break;
                    case nameof(EnumEmitterBlendMode.Alpha):
                        _particleEffect.Emitters[_currentEmitter].BlendMode = BlendMode.Alpha;
                        break;
                    case nameof(EnumEmitterBlendMode.Subtract):
                        _particleEffect.Emitters[_currentEmitter].BlendMode = BlendMode.Subtract;
                        break;
                }
            }
        }

        private void UpdateExecutionStrategy()
        {
            if (_particleEffect.Emitters[_currentEmitter].ModifierExecutionStrategy.ToString() != _dropdownExecutionStrategy.SelectedValue)
            {
                switch (_dropdownExecutionStrategy.SelectedValue)
                {
                    case nameof(EnumExecutionStrategy.Parallel):
                        _particleEffect.Emitters[_currentEmitter].ModifierExecutionStrategy = ModifierExecutionStrategy.Parallel();
                        break;
                    case nameof(EnumExecutionStrategy.Serial):
                        _particleEffect.Emitters[_currentEmitter].ModifierExecutionStrategy = ModifierExecutionStrategy.Serial();
                        break;
                }
            }
        }

        private void UpdateOffset()
        {
            Vector newOffset = Vector.Zero;

            if (_inputOffsetX.Value != null && !_inputOffsetX.Value.Equals("") && !_inputOffsetX.Value.Equals("-")
            && _particleEffect.Emitters[_currentEmitter].Offset.X != float.Parse(_inputOffsetX.Value.Replace(".", ",")))
                newOffset.X = float.Parse(_inputOffsetX.Value.Replace(".", ","));

            if (_inputOffsetY.Value != null && !_inputOffsetY.Value.Equals("") && !_inputOffsetY.Value.Equals("-")
               && _particleEffect.Emitters[_currentEmitter].Offset.Y != float.Parse(_inputOffsetY.Value.Replace(".", ",")))
                newOffset.Y = float.Parse(_inputOffsetY.Value.Replace(".", ","));

            if (newOffset.X != _particleEffect.Emitters[_currentEmitter].Offset.X
                || newOffset.Y != _particleEffect.Emitters[_currentEmitter].Offset.Y)
            {
                _particleEffect.Emitters[_currentEmitter].Offset = newOffset;
            }
        }

        private void UpdateProfile()
        {
            string name = _particleEffect.Emitters[_currentEmitter].Profile.ToString();

            switch (_dropdownProfile.SelectedValue)
            {
                case nameof(EnumEmitterProfiles.BoxFillProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileWidth.Value != null && !_inputProfileWidth.Value.Equals("") && !_inputProfileWidth.Value.Equals("-")
                             && ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width != float.Parse(_inputProfileWidth.Value.Replace(".", ",")))
                        { ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width = float.Parse(_inputProfileWidth.Value.Replace(".", ",")); }

                        if (_inputProfileHeight.Value != null && !_inputProfileHeight.Value.Equals("") && !_inputProfileHeight.Value.Equals("-")
                            && ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height != float.Parse(_inputProfileHeight.Value.Replace(".", ",")))
                        { ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height = float.Parse(_inputProfileHeight.Value.Replace(".", ",")); }
                    }
                    else
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.BoxFill(
                            _particleEffect.Emitters[_currentEmitter].Profile.Width, _particleEffect.Emitters[_currentEmitter].Profile.Height);
                    break;
                case nameof(EnumEmitterProfiles.BoxProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileWidth.Value != null && !_inputProfileWidth.Value.Equals("") && !_inputProfileWidth.Value.Equals("-")
                                             && ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width != float.Parse(_inputProfileWidth.Value.Replace(".", ",")))
                        { ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width = float.Parse(_inputProfileWidth.Value.Replace(".", ",")); }

                        if (_inputProfileHeight.Value != null && !_inputProfileHeight.Value.Equals("") && !_inputProfileHeight.Value.Equals("-")
                            && ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height != float.Parse(_inputProfileHeight.Value.Replace(".", ",")))
                        { ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height = float.Parse(_inputProfileHeight.Value.Replace(".", ",")); }
                    }
                    else
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.Box(100, 100);
                    break;
                case nameof(EnumEmitterProfiles.BoxUniformProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileWidth.Value != null && !_inputProfileWidth.Value.Equals("") && !_inputProfileWidth.Value.Equals("-")
                                             && ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width != float.Parse(_inputProfileWidth.Value.Replace(".", ",")))
                        { ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width = float.Parse(_inputProfileWidth.Value.Replace(".", ",")); }

                        if (_inputProfileHeight.Value != null && !_inputProfileHeight.Value.Equals("") && !_inputProfileHeight.Value.Equals("-")
                            && ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height != float.Parse(_inputProfileHeight.Value.Replace(".", ",")))
                        { ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height = float.Parse(_inputProfileHeight.Value.Replace(".", ",")); }
                    }
                    else
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.BoxUniform(100, 100);
                    break;
                case nameof(EnumEmitterProfiles.CircleProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileRadius.Value != null && !_inputProfileRadius.Value.Equals("") && !_inputProfileRadius.Value.Equals("-")
                     && ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius != float.Parse(_inputProfileRadius.Value.Replace(".", ",")))
                        {
                            ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius = float.Parse(_inputProfileRadius.Value.Replace(".", ","));
                        }

                        if (((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate.ToString() != _dropdownProfileCircleRadiation.SelectedValue)
                        {
                            switch (_dropdownProfileCircleRadiation.SelectedValue)
                            {
                                case nameof(EnumEmitterCircleRadiation.In):
                                    ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.In;
                                    break;
                                case nameof(EnumEmitterCircleRadiation.Out):
                                    ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.Out;
                                    break;
                                case nameof(EnumEmitterCircleRadiation.None):
                                    ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.None;
                                    break;
                            }
                        }
                    }
                    else
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.Circle(_particleEffect.Emitters[_currentEmitter].Profile.Radius, _particleEffect.Emitters[_currentEmitter].Profile.Radiate);
                    break;
                case nameof(EnumEmitterProfiles.LineProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileLength.Value != null && !_inputProfileLength.Value.Equals("") && !_inputProfileLength.Value.Equals("-")
                        && ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Length != float.Parse(_inputProfileLength.Value.Replace(".", ",")))
                        {
                            ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Length = float.Parse(_inputProfileLength.Value.Replace(".", ","));
                        }

                        switch (_dropdownProfileAxis.SelectedValue)
                        {
                            case nameof(EnumEmitterAxis.Up):
                                if (((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis != Axis.Up)
                                {
                                    ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis = Axis.Up;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Down):
                                if (((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis != Axis.Down)
                                {
                                    ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis = Axis.Down;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Left):
                                if (((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis != Axis.Left)
                                {
                                    ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis = Axis.Left;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Right):
                                if (((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis != Axis.Right)
                                {
                                    ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Axis = Axis.Right;
                                }
                                break;
                        }
                    }
                    else
                    {
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.Line(Axis.Down, 100);
                    }
                    break;
                case nameof(EnumEmitterProfiles.PointProfile):
                    _particleEffect.Emitters[_currentEmitter].Profile = Profile.Point();
                    break;
                case nameof(EnumEmitterProfiles.RingProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileRadius.Value != null && !_inputProfileRadius.Value.Equals("") && !_inputProfileRadius.Value.Equals("-")
                        && ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius != float.Parse(_inputProfileRadius.Value.Replace(".", ",")))
                        {
                            ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius = float.Parse(_inputProfileRadius.Value.Replace(".", ","));
                        }

                        if (((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate.ToString() != _dropdownProfileCircleRadiation.SelectedValue)
                        {
                            switch (_dropdownProfileCircleRadiation.SelectedValue)
                            {
                                case nameof(EnumEmitterCircleRadiation.In):
                                    ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.In;
                                    break;
                                case nameof(EnumEmitterCircleRadiation.Out):
                                    ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.Out;
                                    break;
                                case nameof(EnumEmitterCircleRadiation.None):
                                    ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate = CircleProfile.CircleRadiation.None;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.Ring(100, CircleRadiation.In);
                    }
                    break;
                case nameof(EnumEmitterProfiles.SprayProfile):
                    if (name.Equals(_dropdownProfile.SelectedValue))
                    {
                        if (_inputProfileSpread.Value != null && !_inputProfileSpread.Value.Equals("") && !_inputProfileSpread.Value.Equals("-")
                         && ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Spread != float.Parse(_inputProfileSpread.Value.Replace(".", ",")))
                        {
                            ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Spread = float.Parse(_inputProfileSpread.Value.Replace(".", ","));
                        }

                        switch (_dropdownProfileAxis.SelectedValue)
                        {
                            case nameof(EnumEmitterAxis.Up):
                                if (((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction != Axis.Up)
                                {
                                    ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction = Axis.Up;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Down):
                                if (((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction != Axis.Down)
                                {
                                    ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction = Axis.Down;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Left):
                                if (((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction != Axis.Left)
                                {
                                    ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction = Axis.Left;
                                }
                                break;
                            case nameof(EnumEmitterAxis.Right):
                                if (((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction != Axis.Right)
                                {
                                    ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Direction = Axis.Right;
                                }
                                break;
                        }
                    }
                    else
                    {
                        _particleEffect.Emitters[_currentEmitter].Profile = Profile.Spray(Axis.Down, 100);
                    }
                    break;
            }
        }

        private void UpdateReclaimFrequency()
        {
            if (_inputReclaimFrequency.Value != null && !_inputReclaimFrequency.Value.Equals("") && !_inputReclaimFrequency.Value.Equals("-")
                 && _particleEffect.Emitters[_currentEmitter].ReclaimFrequency != float.Parse(_inputReclaimFrequency.Value.Replace(".", ",")))
                _particleEffect.Emitters[_currentEmitter].ReclaimFrequency = float.Parse(_inputReclaimFrequency.Value.Replace(".", ","));
        }

        private void UpdateTimeMilleseconds()
        {
            if (_inputTimeMilleseconds.Value != null && !_inputTimeMilleseconds.Value.Equals("") && !_inputTimeMilleseconds.Value.Equals("-")
                    && _particleEffect.Emitters[_currentEmitter].Term.TotalMilliseconds != int.Parse(_inputTimeMilleseconds.Value))
                _particleEffect.Emitters[_currentEmitter].Term = TimeSpan.FromMilliseconds(double.Parse(_inputTimeMilleseconds.Value));
        }

        private void UpdateCapacity()
        {
            if (_inputCapacity.Value != null && !_inputCapacity.Value.Equals("") && !_inputCapacity.Value.Equals("-")
                 && _particleEffect.Emitters[_currentEmitter].Capacity != int.Parse(_inputCapacity.Value))
                _particleEffect.Emitters[_currentEmitter].Capacity = int.Parse(_inputCapacity.Value);
        }

        private void UpdateColor()
        {
            _parametersParagraphColorH.Text = "HUE " + _parametersSliderColorH.Value;
            _parametersParagraphColorS.Text = "Saturation " + _parametersSliderColorS.Value + "%";
            _parametersParagraphColorL.Text = "Lightness " + _parametersSliderColorL.Value + "%";

            _particleEffect.Emitters[_currentEmitter].Parameters.Colour = new VenusParticleEngine.Core.Colour(
               _parametersSliderColorH.Value,
               (float)_parametersSliderColorS.Value / 100,
               (float)_parametersSliderColorL.Value / 100);
        }

        private void UpdateOpacity()
        {
            _parametersParagraphOpacity.Text = "Opacity " + _parametersSliderOpacity.Value + "%";
            _particleEffect.Emitters[_currentEmitter].Parameters.Opacity = (float)_parametersSliderOpacity.Value / 100;
        }

        private void UpdateQuantity()
        {
            if (_inputQuantity.Value != null && !_inputQuantity.Value.Equals("") && !_inputQuantity.Value.Equals("-")
                && _particleEffect.Emitters[_currentEmitter].Parameters.Quantity != int.Parse(_inputQuantity.Value))
            {
                _particleEffect.Emitters[_currentEmitter].Parameters.Quantity = int.Parse(_inputQuantity.Value);
            }

        }

        private void UpdateSpeed()
        {
            float min = _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Min;
            float max = _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Max;

            if (_inputSpeedMin.Value != null && !_inputSpeedMin.Value.Equals("") && !_inputSpeedMin.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Min != float.Parse(_inputSpeedMin.Value.Replace(".", ",")))
            {
                min = float.Parse(_inputSpeedMin.Value.Replace(".", ","));
            }

            if (_inputSpeedMax.Value != null && !_inputSpeedMax.Value.Equals("") && !_inputSpeedMax.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Max != float.Parse(_inputSpeedMax.Value.Replace(".", ",")))
            {
                max = float.Parse(_inputSpeedMax.Value.Replace(".", ","));
            }

            _particleEffect.Emitters[_currentEmitter].Parameters.Speed = new RangeF(min, max);
        }

        private void UpdateScale()
        {
            if (_inputScale.Value != null && !_inputScale.Value.Equals("") && !_inputScale.Value.Equals("-"))
                _particleEffect.Emitters[_currentEmitter].Parameters.Scale = float.Parse(_inputScale.Value.Replace(".", ","));
        }

        private void UpdateRotation()
        {
            float min = _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Min;
            float max = _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Max;

            if (_inputRotationMin.Value != null && !_inputRotationMin.Value.Equals("") && !_inputRotationMin.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Min != float.Parse(_inputRotationMin.Value.Replace(".", ",")))
            {
                min = float.Parse(_inputRotationMin.Value.Replace(".", ","));
            }

            if (_inputRotationMax.Value != null && !_inputRotationMax.Value.Equals("") && !_inputRotationMax.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Max != float.Parse(_inputRotationMax.Value.Replace(".", ",")))
            {
                max = float.Parse(_inputRotationMax.Value.Replace(".", ","));
            }

            _particleEffect.Emitters[_currentEmitter].Parameters.Rotation = new RangeF(min, max);
        }

        private void UpdateMass()
        {
            float min = _particleEffect.Emitters[_currentEmitter].Parameters.Mass.Min;
            float max = _particleEffect.Emitters[_currentEmitter].Parameters.Mass.Max;

            if (_inputMassMin.Value != null && !_inputMassMin.Value.Equals("") && !_inputMassMin.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Min != float.Parse(_inputMassMin.Value.Replace(".", ",")))
            {
                min = float.Parse(_inputMassMin.Value.Replace(".", ","));
            }

            if (_inputMassMax.Value != null && !_inputMassMax.Value.Equals("") && !_inputMassMax.Value.Equals("-")
              && _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Max != float.Parse(_inputMassMax.Value.Replace(".", ",")))
            {
                max = float.Parse(_inputMassMax.Value.Replace(".", ","));
            }

            _particleEffect.Emitters[_currentEmitter].Parameters.Mass = new RangeF(min, max);
        }

        private void UpdateDropdownProfileAxisFromParticle()
        {
            if ((_particleEffect.Emitters[_currentEmitter].Profile).Axis == Axis.Up)
            {
                _dropdownProfileAxis.SelectedValue = "Up";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Axis == Axis.Down)
            {
                _dropdownProfileAxis.SelectedValue = "Down";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Axis == Axis.Left)
            {
                _dropdownProfileAxis.SelectedValue = "Left";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Axis == Axis.Right)
            {
                _dropdownProfileAxis.SelectedValue = "Right";
            }
        }
        private void UpdateDropdownProfileDirectionFromParticle()
        {
            if ((_particleEffect.Emitters[_currentEmitter].Profile).Direction == Axis.Up)
            {
                _dropdownProfileAxis.SelectedValue = "Up";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Direction == Axis.Down)
            {
                _dropdownProfileAxis.SelectedValue = "Down";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Direction == Axis.Left)
            {
                _dropdownProfileAxis.SelectedValue = "Left";
            }
            else if ((_particleEffect.Emitters[_currentEmitter].Profile).Direction == Axis.Right)
            {
                _dropdownProfileAxis.SelectedValue = "Right";
            }
        }

        private void RefreshEmitterProfile()
        {
            _inputProfileWidth.Enabled = false;
            _inputProfileHeight.Enabled = false;
            _inputProfileRadius.Enabled = false;
            _dropdownProfileCircleRadiation.Enabled = false;
            _dropdownProfileAxis.Enabled = false;
            _inputProfileLength.Enabled = false;
            _inputProfileSpread.Enabled = false;

            switch (_dropdownProfile.SelectedValue)
            {
                case nameof(EnumEmitterProfiles.BoxProfile):
                    _inputProfileWidth.Enabled = true;
                    _inputProfileHeight.Enabled = true;

                    _inputProfileHeight.Value = ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height.ToString();
                    _inputProfileWidth.Value = ((BoxProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width.ToString();
                    break;
                case nameof(EnumEmitterProfiles.BoxFillProfile):
                    _inputProfileWidth.Enabled = true;
                    _inputProfileHeight.Enabled = true;

                    _inputProfileHeight.Value = ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height.ToString();
                    _inputProfileWidth.Value = ((BoxFillProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width.ToString();
                    break;
                case nameof(EnumEmitterProfiles.BoxUniformProfile):
                    _inputProfileWidth.Enabled = true;
                    _inputProfileHeight.Enabled = true;

                    _inputProfileHeight.Value = ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Height.ToString();
                    _inputProfileWidth.Value = ((BoxUniformProfile)_particleEffect.Emitters[_currentEmitter].Profile).Width.ToString();
                    break;
                case nameof(EnumEmitterProfiles.CircleProfile):
                    _inputProfileRadius.Enabled = true;
                    _dropdownProfileCircleRadiation.Enabled = true;
                    _inputProfileRadius.Value = ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius.ToString();
                    _dropdownProfileCircleRadiation.SelectedValue = ((CircleProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate.ToString();
                    break;
                case nameof(EnumEmitterProfiles.LineProfile):
                    _dropdownProfileAxis.Enabled = true;
                    _inputProfileLength.Enabled = true;

                    _inputProfileLength.Value = ((LineProfile)_particleEffect.Emitters[_currentEmitter].Profile).Length.ToString();

                    UpdateDropdownProfileAxisFromParticle();
                    break;
                case nameof(EnumEmitterProfiles.PointProfile):
                    break;
                case nameof(EnumEmitterProfiles.RingProfile):
                    _inputProfileRadius.Enabled = true;
                    _dropdownProfileCircleRadiation.Enabled = true;

                    _inputProfileRadius.Value = ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radius.ToString();
                    _dropdownProfileCircleRadiation.SelectedValue = ((RingProfile)_particleEffect.Emitters[_currentEmitter].Profile).Radiate.ToString();
                    break;
                case nameof(EnumEmitterProfiles.SprayProfile):
                    _dropdownProfileAxis.Enabled = true;
                    _inputProfileSpread.Enabled = true;

                    _inputProfileSpread.Value = ((SprayProfile)_particleEffect.Emitters[_currentEmitter].Profile).Spread.ToString();

                    UpdateDropdownProfileDirectionFromParticle();
                    break;
            }
        }

        private void RefreshEmitter()
        {
            if (_particleEffect.Emitters.Count < 1 || _currentEmitter == null) return;
            if (!_particleEffect.Emitters.ContainsKey(_currentEmitter)) return;

            _parametersSliderColorH.Value = (int)_particleEffect.Emitters[_currentEmitter].Parameters.Colour.Min.H;
            _parametersSliderColorS.Value = ((int)(_particleEffect.Emitters[_currentEmitter].Parameters.Colour.Min.S * 100));
            _parametersSliderColorL.Value = ((int)(_particleEffect.Emitters[_currentEmitter].Parameters.Colour.Min.L * 100));
            _parametersSliderOpacity.Value = ((int)(_particleEffect.Emitters[_currentEmitter].Parameters.Opacity.Min * 100));

            _inputCapacity.Value = _particleEffect.Emitters[_currentEmitter].Capacity.ToString();
            _inputTimeMilleseconds.Value = _particleEffect.Emitters[_currentEmitter].Term.TotalMilliseconds.ToString();
            _dropdownProfile.SelectedValue = _particleEffect.Emitters[_currentEmitter].Profile.ToString();
            RefreshEmitterProfile();
            _dropdownBlendMode.SelectedValue = _particleEffect.Emitters[_currentEmitter].BlendMode.ToString();
            _inputReclaimFrequency.Value = _particleEffect.Emitters[_currentEmitter].ReclaimFrequency.ToString();
            _dropdownExecutionStrategy.SelectedValue = _particleEffect.Emitters[_currentEmitter].ModifierExecutionStrategy.ToString();
            _inputOffsetX.Value = _particleEffect.Emitters[_currentEmitter].Offset.X.ToString();
            _inputOffsetY.Value = _particleEffect.Emitters[_currentEmitter].Offset.Y.ToString();
            _inputQuantity.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Quantity.Min.ToString();
            _inputSpeedMin.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Min.ToString();
            _inputSpeedMax.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Speed.Max.ToString();
            _inputScale.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Scale.Min.ToString();
            _inputRotationMin.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Min.ToString();
            _inputRotationMax.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Rotation.Max.ToString();
            _inputMassMin.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Mass.Min.ToString();
            _inputMassMax.Value = _particleEffect.Emitters[_currentEmitter].Parameters.Mass.Max.ToString();

            _modifierCircleContainer.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierColourInterpolator2.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierDrag.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierHueInterpolator2.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierLinearGravity.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierOpacityFastFade.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierOpacityInterpolator2.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierOpacityInterpolator3.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierRectContainer.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierRectLoopContainer.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierRectLoopContainer.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierRotation.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierScaleInterpolator2.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierVelocityColour.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierVelocityColourInfinite.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierVelocityHue.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
            _modifierVortex.Refresh(_particleEffect.Emitters[_currentEmitter].Modifiers);
        }
    }
}