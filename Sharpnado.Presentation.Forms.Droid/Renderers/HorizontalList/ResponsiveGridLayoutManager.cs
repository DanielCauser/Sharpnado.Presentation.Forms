﻿using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;

using Sharpnado.Presentation.Forms.Droid.Helpers;

using Xamarin.Forms;

using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _space;
        private readonly int _leftPadding;
        private readonly int _topPadding;
        private readonly int _rightPadding;
        private readonly int _bottomPadding;

        public SpaceItemDecoration(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SpaceItemDecoration(int spaceDp, Thickness paddingDp)
        {
            _space = PlatformHelper.DpToPixels(spaceDp);
            _leftPadding = PlatformHelper.DpToPixels(paddingDp.Left);
            _topPadding = PlatformHelper.DpToPixels(paddingDp.Top);
            _rightPadding = PlatformHelper.DpToPixels(paddingDp.Right);
            _bottomPadding = PlatformHelper.DpToPixels(paddingDp.Bottom);
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int left, top, right, bottom;
            left = right = top = bottom = _space / 2;

            bool isViewEdgeLeft = false;
            bool isViewEdgeTop = false;
            bool isViewEdgeRight = false;
            bool isViewEdgeBottom = false;

            int viewPosition = parent.GetChildAdapterPosition(view);
            int viewCount = parent.GetAdapter().ItemCount;

            if (parent.GetLayoutManager() is ResponsiveGridLayoutManager responsiveGridLayout)
            {
                int spanCount = responsiveGridLayout.SpanCount;
                isViewEdgeLeft = viewPosition % spanCount == 0;
                isViewEdgeTop = viewPosition < spanCount;
                isViewEdgeRight = viewPosition % spanCount == spanCount - 1;
                isViewEdgeBottom = viewPosition / spanCount == viewCount - 1 / spanCount;
            }
            else if (parent.GetLayoutManager() is LinearLayoutManager)
            {
                isViewEdgeLeft = viewPosition == 0;
                isViewEdgeTop = isViewEdgeBottom = true;
                isViewEdgeRight = viewPosition == viewCount - 1;
            }

            if (isViewEdgeLeft)
            {
                left = _leftPadding > _space ? _leftPadding : _space;
            }

            if (isViewEdgeRight)
            {
                right = _rightPadding > _space ? _rightPadding : _space;
            }

            if (isViewEdgeTop)
            {
                top = _topPadding > 0 ? _topPadding : _space;
            }

            if (isViewEdgeBottom)
            {
                bottom = _bottomPadding > 0 ? _bottomPadding : _space;
            }

            outRect.Left = left;
            outRect.Top = top;
            outRect.Right = right;
            outRect.Bottom = bottom;
        }
    }

    public class ResponsiveGridLayoutManager : GridLayoutManager
    {
        private readonly Context _context;
        private readonly int _itemWidthDp;
        private readonly int _marginDp;

        public ResponsiveGridLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ResponsiveGridLayoutManager(Context context, int itemWidthDp, int marginDp = 0)
            : base(context, 1)
        {
            _context = context;
            _itemWidthDp = itemWidthDp;
            _marginDp = marginDp;
        }

        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            ComputeSpanCount(Width);
            base.OnLayoutChildren(recycler, state);
        }

        private void ComputeSpanCount(int recyclerWidth)
        {
            int spanCount = recyclerWidth / PlatformHelper.DpToPixels(_itemWidthDp + 2 * _marginDp);
            SpanCount = spanCount;
        }
    }
}