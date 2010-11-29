// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Automation;
using System.Windows.Input;

public class Helpers
{
    public static void MoveToAndClick(AutomationElement element)
    {
        System.Windows.Point winPoint = element.GetClickablePoint();
        System.Drawing.Point drawingPoint = new System.Drawing.Point((int)winPoint.X, (int)winPoint.Y);
        Microsoft.Test.Input.Mouse.MoveTo(drawingPoint);
        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);

        if (AutomationElement.FocusedElement != element)
        {
            throw new ApplicationException("Failed to focus desired UI element by clicking on it.");
        }
    }
}
