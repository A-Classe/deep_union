using System.GameProgress;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace Debug
{
    public sealed class DebugToolPage : DefaultDebugPageBase
    {
        private LabelObserver<StageProgressObserver> progressObserver;
        private StageProgressObserver stageProgress;
        protected override string Title => "Debug Tool";

        private void Update()
        {
            progressObserver.Update(stageProgress);
            RefreshDataAt(progressObserver.CellId);

            Reload();
        }

        public void SetUp(StageProgressObserver stageProgress)
        {
            this.stageProgress = stageProgress;

            progressObserver =
                LabelObserver<StageProgressObserver>.Create(this, observer =>
                {
                    var progress = observer.Progress * 100f;
                    return $"GameProgress: {progress:0}%";
                });
        }
    }
}