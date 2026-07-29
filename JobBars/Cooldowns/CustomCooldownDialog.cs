using Dalamud.Bindings.ImGui;
using JobBars.Cooldowns.Manager;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Numerics;

namespace JobBars.Cooldowns {
    public class CustomCooldownDialog : GenericDialog {
        private enum CustomCooldownType {
            Buff,
            Action
        }

        private static readonly JobIds[] JobOptions = ( JobIds[] )Enum.GetValues( typeof( JobIds ) );
        private JobIds SelectedJob = JobIds.OTHER;

        private CustomCooldownType CustomTriggerType = CustomCooldownType.Action;
        private float CustomCD = 30;
        private float CustomDuration = 0;

        private readonly ItemSelector CustomTriggerAction = new( "觸發條件", "##CustomCD_Action", UiHelper.ActionList );
        private readonly ItemSelector CustomTriggerBuff = new( "觸發條件", "##CustomCD_Buff", UiHelper.StatusList );
        private readonly ItemSelector CustomIcon = new( "圖示", "##CustomCD_Icon", UiHelper.ActionList );

        private static CooldownManager Manager => JobBars.CooldownManager;


        public CustomCooldownDialog() : base( "自訂冷卻###Custom Cooldown" ) { }

        public override void DrawBody() {
            var id = "##CustomCooldown";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            if( JobBars.DrawCombo( JobOptions, SelectedJob, "職業", id, out var newSelectedJob ) ) {
                SelectedJob = newSelectedJob;
            }

            if( ImGui.BeginCombo( "##CustomCD_Type", $"{CustomTriggerType}", ImGuiComboFlags.HeightLargest ) ) {
                if( ImGui.Selectable( "技能###Action", CustomTriggerType == CustomCooldownType.Action ) ) CustomTriggerType = CustomCooldownType.Action;
                if( ImGui.Selectable( "增益狀態###Buff", CustomTriggerType == CustomCooldownType.Buff ) ) CustomTriggerType = CustomCooldownType.Buff;
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text( "觸發類型" );

            if( CustomTriggerType == CustomCooldownType.Action ) CustomTriggerAction.Draw();
            else CustomTriggerBuff.Draw();

            CustomIcon.Draw();

            ImGui.InputFloat( "冷卻時間###Cooldown", ref CustomCD );
            ImGui.InputFloat( "持續時間（0 = 立即）###Duration (0 = instant)", ref CustomDuration );

            var selected = CustomTriggerType == CustomCooldownType.Action ? CustomTriggerAction.GetSelected() : CustomTriggerBuff.GetSelected();
            var icon = CustomIcon.GetSelected();

            ImGui.EndChild();

            if( icon.Data.Id != 0 && selected.Data.Id != 0 ) {
                if( ImGui.Button( "+ 新增###+ Add" ) ) {
                    var newName = $"{selected.Name} - Custom ({UiHelper.Localize( SelectedJob )})";
                    var newProps = new CooldownProps {
                        CD = CustomCD,
                        Duration = CustomDuration,
                        Icon = ( ActionIds )icon.Data.Id,
                        Triggers = [selected.Data]
                    };
                    Manager.AddCustomCooldown( SelectedJob, newName, newProps );
                    Manager.ResetUi();
                }
            }
            else {
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
                ImGui.Button( "+ 新增###+ Add" );
                ImGui.PopStyleVar();
            }
        }

        public void Show( JobIds job ) {
            SelectedJob = job;
            Show();
        }
    }
}
