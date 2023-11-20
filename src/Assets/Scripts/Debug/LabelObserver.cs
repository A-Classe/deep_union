using System;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace Debug
{
    internal readonly struct LabelObserver<T>
    {
        public readonly int CellId;
        private readonly LabelCellModel cellModel;
        private readonly Func<T, string> getText;

        private LabelObserver(int cellId, LabelCellModel cellModel, Func<T, string> getText)
        {
            CellId = cellId;
            this.cellModel = cellModel;
            this.getText = getText;
        }

        public static LabelObserver<T> Create(DefaultDebugPageBase pageBase, Func<T, string> getTextFunc,
            int priority = 0)
        {
            var model = new LabelCellModel(false);
            var cellId = pageBase.AddLabel(model, priority);

            return new LabelObserver<T>(cellId, model, getTextFunc);
        }

        public void Update(T data)
        {
            cellModel.CellTexts.Text = getText(data);
        }
    }
}