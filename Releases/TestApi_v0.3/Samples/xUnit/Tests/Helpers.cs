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
        Microsoft.Test.Input.Mouse.Click(MouseButton.Left);

        if (AutomationElement.FocusedElement != element)
        {
            throw new ApplicationException("Failed to focus desired UI element by clicking on it.");
        }
    }
}
