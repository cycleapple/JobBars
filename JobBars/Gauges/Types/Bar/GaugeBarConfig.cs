namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }
        public bool Vertical { get; private set; }

        public GaugeBarConfig( string name ) : base( name ) {
            ShowText = JobBars.Configuration.GaugeShowText.Get( Name );
            SwapText = JobBars.Configuration.GaugeSwapText.Get( Name );
            Vertical = JobBars.Configuration.GaugeVertical.Get( Name );
        }

        public override void Draw( string id, ref bool newVisual, ref bool reset ) {
            if( JobBars.Configuration.GaugeShowText.Draw( $"顯示文字{id}", Name, ShowText, out var newShowText ) ) {
                ShowText = newShowText;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeSwapText.Draw( $"交換文字位置{id}", Name, SwapText, out var newSwapText ) ) {
                SwapText = newSwapText;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeVertical.Draw( $"垂直顯示{id}", Name, Vertical, out var newVertical ) ) {
                Vertical = newVertical;
                newVisual = true;
            }
        }
    }
}
