using Dalamud.Bindings.ImGui;
using JobBars.Data;
using JobBars.Nodes.Builder;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        public bool LOCKED = true;

        private readonly InfoBox<BuffManager> PositionInfoBox = new() {
            Label = "Position",
            ContentsAction = ( BuffManager manager ) => {
                ImGui.Checkbox( "鎖定位置" + manager.Id, ref manager.LOCKED );

                ImGui.SetNextItemWidth( 25f );
                if( ImGui.InputInt( "Buffs per line" + manager.Id, ref JobBars.Configuration.BuffHorizontal, 0 ) ) {
                    JobBars.Configuration.Save();
                    JobBars.NodeBuilder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "由右至左" + manager.Id, ref JobBars.Configuration.BuffRightToLeft ) ) {
                    JobBars.Configuration.Save();
                    JobBars.NodeBuilder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "由下至上" + manager.Id, ref JobBars.Configuration.BuffBottomToTop ) ) {
                    JobBars.Configuration.Save();
                    JobBars.NodeBuilder.BuffRoot.Update();
                }

                if( ImGui.Checkbox( "正方形增益圖示" + manager.Id, ref JobBars.Configuration.BuffSquare ) ) {
                    JobBars.Configuration.Save();
                    JobBars.NodeBuilder.BuffRoot.Update();
                }

                if( ImGui.InputFloat( "Scale" + manager.Id, ref JobBars.Configuration.BuffScale ) ) {
                    UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                var pos = JobBars.Configuration.BuffPosition;
                if( ImGui.InputFloat2( "Position" + manager.Id, ref pos ) ) {
                    SetBuffPosition( pos );
                }
            }
        };

        private readonly InfoBox<BuffManager> HideWhenInfoBox = new() {
            Label = "Hide When",
            ContentsAction = ( BuffManager manager ) => {
                if( ImGui.Checkbox( "非戰鬥中", ref JobBars.Configuration.BuffHideOutOfCombat ) ) JobBars.Configuration.Save();
                if( ImGui.Checkbox( "收起武器時", ref JobBars.Configuration.BuffHideWeaponSheathed ) ) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if( ImGui.Checkbox( "啟用增益列" + Id, ref JobBars.Configuration.BuffBarEnabled ) ) {
                JobBars.Configuration.Save();
                ResetUi();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw( this );
            HideWhenInfoBox.Draw( this );

            ImGui.SetNextItemWidth( 50f );
            if( ImGui.InputFloat( "Hide buffs with cooldown above" + Id, ref JobBars.Configuration.BuffDisplayTimer ) ) JobBars.Configuration.Save();

            if( ImGui.Checkbox( "顯示隊員的增益狀態", ref JobBars.Configuration.BuffIncludeParty ) ) {
                JobBars.Configuration.Save();
                ResetUi();
            }

            if( ImGui.Checkbox( "使用細增益邊框", ref JobBars.Configuration.BuffThinBorder ) ) {
                JobBars.Configuration.Save();
                JobBars.NodeBuilder.BuffRoot.Update();
            }

            ImGui.SetNextItemWidth( 50f );
            if( ImGui.InputFloat( "Opacity when on cooldown" + Id, ref JobBars.Configuration.BuffOnCDOpacity ) ) JobBars.Configuration.Save();

            ImGui.SetNextItemWidth( 100f );
            if( ImGui.InputInt( "Buff text size", ref JobBars.Configuration.BuffTextSize_v2 ) ) {
                if( JobBars.Configuration.BuffTextSize_v2 <= 0 ) JobBars.Configuration.BuffTextSize_v2 = 1;
                if( JobBars.Configuration.BuffTextSize_v2 > 255 ) JobBars.Configuration.BuffTextSize_v2 = 255;
                JobBars.Configuration.Save();
                JobBars.NodeBuilder.BuffRoot.Update();
            }
        }

        protected override void DrawItem( BuffConfig[] item, JobIds _ ) {
            var reset = false;
            foreach( var buff in item ) buff.Draw( Id, ref reset );
            if( reset ) ResetUi();
        }

        public void DrawPositionBox() {
            if( LOCKED ) return;
            if( JobBars.DrawPositionView( "Buff Bar##BuffPosition", JobBars.Configuration.BuffPosition, out var pos ) ) SetBuffPosition( pos );
        }

        private static void SetBuffPosition( Vector2 pos ) {
            JobBars.SetWindowPosition( "Buff Bar##BuffPosition", pos );
            JobBars.Configuration.BuffPosition = pos;
            JobBars.Configuration.Save();
            NodeBuilder.SetPositionGlobal( JobBars.NodeBuilder.BuffRoot, JobBars.Configuration.BuffPosition );
        }
    }
}
