using Dalamud.Bindings.ImGui;
using JobBars.Data;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private readonly InfoBox<CooldownManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = ( CooldownManager manager ) => {
                if( ImGui.Checkbox( "靠左對齊" + manager.Id, ref JobBars.Configuration.CooldownsLeftAligned ) ) {
                    JobBars.Configuration.Save();
                    manager.ResetUi();
                }

                if( ImGui.InputFloat( "Scale" + manager.Id, ref JobBars.Configuration.CooldownScale ) ) {
                    UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                if( ImGui.InputFloat2( "Position" + manager.Id, ref JobBars.Configuration.CooldownPosition ) ) {
                    UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                if( ImGui.InputFloat( "Line height" + manager.Id, ref JobBars.Configuration.CooldownsSpacing ) ) {
                    UpdatePositionScale();
                    JobBars.Configuration.Save();
                }
            }
        };

        private readonly InfoBox<CooldownManager> ShowIconInfoBox = new() {
            Label = "Show Icons When",
            ContentsAction = ( CooldownManager manager ) => {
                if( ImGui.Checkbox( "預設" + manager.Id, ref JobBars.Configuration.CooldownsStateShowDefault ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "作用中" + manager.Id, ref JobBars.Configuration.CooldownsStateShowRunning ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "冷卻中" + manager.Id, ref JobBars.Configuration.CooldownsStateShowOnCD ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "可使用" + manager.Id, ref JobBars.Configuration.CooldownsStateShowOffCD ) ) JobBars.Configuration.Save();
            }
        };

        private readonly InfoBox<CooldownManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = ( CooldownManager manager ) => {
                if( ImGui.Checkbox( "非戰鬥中", ref JobBars.Configuration.CooldownsHideOutOfCombat ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "收起武器時", ref JobBars.Configuration.CooldownsHideWeaponSheathed ) ) JobBars.Configuration.Save();
            }
        };

        private readonly CustomCooldownDialog CustomCooldownDialog = new();

        protected override void DrawHeader() {
            CustomCooldownDialog.Draw();

            if( ImGui.Checkbox( "啟用冷卻時間" + Id, ref JobBars.Configuration.CooldownsEnabled ) ) {
                JobBars.Configuration.Save();
                ResetUi();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw( this );
            ShowIconInfoBox.Draw( this );
            HideWhenInfoBox.Draw( this );

            if( ImGui.Checkbox( "隱藏作用中增益的文字" + Id, ref JobBars.Configuration.CooldownsHideActiveBuffDuration ) ) JobBars.Configuration.Save();

            if( ImGui.Checkbox( "顯示隊員的冷卻時間" + Id, ref JobBars.Configuration.CooldownsShowPartyMembers ) ) {
                JobBars.Configuration.Save();
                ResetUi();
            }

            ImGui.SetNextItemWidth( 50f );
            if( ImGui.InputFloat( "Opacity when on cooldown" + Id, ref JobBars.Configuration.CooldownsOnCDOpacity ) ) JobBars.Configuration.Save();
        }

        protected override void DrawItem( CooldownConfig[] item, JobIds job ) {
            var reset = false;
            foreach( var cooldown in item ) cooldown.Draw( Id, false, ref reset );

            // Delete custom
            if( CustomCooldowns.TryGetValue( job, out var customCooldowns ) ) {
                foreach( var custom in customCooldowns ) {
                    if( custom.Draw( Id, true, ref reset ) ) {
                        DeleteCustomCooldown( job, custom );
                        reset = true;
                        break;
                    }
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );
            if( ImGui.Button( $"+ 新增自訂冷卻{Id}" ) ) CustomCooldownDialog.Show( job );

            if( reset ) ResetUi();
        }
    }
}
