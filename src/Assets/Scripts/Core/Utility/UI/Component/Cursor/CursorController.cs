using System;
using System.Collections.Generic;
using System.Linq;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Component.Cursor
{
    public class CursorController<T>: AnimationBehaviour where T : Enum
    {
        private Dictionary<T, RectTransform> points = new ();

        [SerializeField] private AnimateObject left;
        [SerializeField] private AnimateObject right;

        private T currentPoint;

        private bool isInitialize = false;

        public void AddPoint(T key, RectTransform rectT)
        {
            points.Add(key, rectT);
        }

        public void SetPoint(T key)
        {
            if (currentPoint.Equals(key) && isInitialize) return;
            isInitialize = true;

            if (!points.Keys.Contains(key)) return;
            currentPoint = key;
            Animated(points[key]);
        }

        private void Animated(RectTransform rect)
        {
            var position = rect.position;
            float leftX = position.x - rect.rect.width / 2f;
            float rightX = position.x + rect.rect.width / 2f;

            var leftT = left.transform.position;
            left.OnCancel();
            left.Animation(
                this.SlideTo(new Vector2(leftX - leftT.x, position.y - leftT.y), Easings.CircIn(0.3f)),
                new AnimationListener()
                {
                    OnFinished = () =>
                    {
                        var pos = position;
                        pos.x = leftX;
                        left.transform.position = pos;
                    }
                }
            );
            var rightT = right.transform.position;
            right.OnCancel();
            right.Animation(
                this.SlideTo(new Vector2(rightX - rightT.x, position.y - rightT.y), Easings.CircIn(0.3f)),
                new AnimationListener()
                {
                    OnFinished = () =>
                    {
                        var pos = position;
                        pos.x = rightX;
                        right.transform.position = pos;
                    }
                }
            );
        }
    }
}