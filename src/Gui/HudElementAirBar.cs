using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace OxygenNotIncluded.Gui
{
    public class HudElementAirBar : HudElement
    {
        private GuiElementStatbar _statbar;

        public override double InputOrder => 1.0;

        public HudElementAirBar(ICoreClientAPI capi) : base(capi)
        {
            capi.Event.RegisterGameTickListener(OnGameTick, 20);
            capi.Event.RegisterGameTickListener(OnFlashStatbar, 1000);
        }

        private void OnGameTick(float dt)
        {
            UpdateAir();
        }

        private void OnFlashStatbar(float dt)
        {
            var airTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("oni:air");

            if (airTree != null && _statbar != null)
            {
                _statbar.ShouldFlash = _statbar.GetValue() < OxygenNotIncludedMod.Config.MaxAir * 0.2f;
            }
        }

        private void UpdateAir()
        {
            if (_statbar == null) return;

            var airTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("oni:air");

            var currentAir = airTree?.TryGetFloat("current-air");

            if (!currentAir.HasValue) return;

            var maxAir = OxygenNotIncludedMod.Config.MaxAir;
            var lineInterval = maxAir * 0.1f;

            _statbar.SetLineInterval(lineInterval);
            _statbar.SetValues(currentAir.Value, 0.0f, maxAir);
        }

        private void ComposeGuis()
        {
            const float statsBarParentWidth = 850f;
            const float statsBarWidth = statsBarParentWidth * 0.41f;

            double[] airBarColor = {0, 0.4, 0.5, 0.5};

            var airTree = capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("oni:air");

            var statsBarBounds = new ElementBounds()
            {
                Alignment = EnumDialogArea.CenterBottom,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = statsBarParentWidth,
                fixedHeight = 100.0
            }.WithFixedAlignmentOffset(0.0, 5.0);

            var isRight = OxygenNotIncludedMod.Config.AirBarHorizontalAlignmentRight;
            var alignment = isRight ? EnumDialogArea.RightTop : EnumDialogArea.LeftTop;
            var alignmentOffsetX = isRight ? -2.0 : 1.0;

            var airBarBounds = ElementStdBounds.Statbar(alignment, statsBarWidth)
                .WithFixedAlignmentOffset(alignmentOffsetX, OxygenNotIncludedMod.Config.AirBarVerticalAlignmentOffset)
                .WithFixedHeight(10.0);

            var airBarParentBounds = statsBarBounds.FlatCopy().FixedGrow(0.0, 20.0);

            var composer = capi.Gui.CreateCompo("oni:statbar", airBarParentBounds);

            _statbar = new GuiElementStatbar(composer.Api, airBarBounds, airBarColor, isRight);

            composer
                .BeginChildElements(statsBarBounds)
                .AddIf(airTree != null)
                .AddInteractiveElement(_statbar, "airstatsbar")
                .EndIf()
                .EndChildElements()
                .Compose();

            Composers["oni:airbar"] = composer;

            TryOpen();
        }

        public override void OnOwnPlayerDataReceived()
        {
            ComposeGuis();
            UpdateAir();
        }

        public override void OnRenderGUI(float deltaTime)
        {
            if (capi.World.Player.WorldData.CurrentGameMode == EnumGameMode.Spectator) return;

            base.OnRenderGUI(deltaTime);
        }

        public override bool TryClose() => false;

        public override bool ShouldReceiveKeyboardEvents() => false;

        public override bool Focusable => false;
    }
}
