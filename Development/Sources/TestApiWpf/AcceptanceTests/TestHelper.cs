// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.AcceptanceTests
{
    public static class TestHelper
    {
        #region GetVisualChild

        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        public static T GetVisualChild<T>(Visual parent, int index) where T : Visual
        {
            T child = default(T);

            int encounter = 0;
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                child = v as T;
                if (child != null)
                {
                    if (encounter == index)
                        break;
                    encounter++;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return child;
        }

        public static bool VisualChildExists(Visual parent, DependencyObject visualToFind)
        {
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                DependencyObject child = v as DependencyObject;
                if (child != null)
                {
                    if (child == visualToFind)
                        return true;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return false;
        }

        #endregion GetVisualChild
    }
}
