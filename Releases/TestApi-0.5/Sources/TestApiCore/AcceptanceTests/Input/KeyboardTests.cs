// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Microsoft.Test.Input;
using Xunit;
using Xunit.Extensions;


namespace Microsoft.Test.AcceptanceTests.Input
{
    public class KeyboardTests : IDisposable
    {
        #region Private Fields

        private Process _process;
        private Key[] _alphanumericKeys = new Key[]
            {
                Key.A,
                Key.B,
                Key.C,
                Key.D,
                Key.E,
                Key.F,
                Key.G,
                Key.H,
                Key.I,
                Key.J,
                Key.K,
                Key.L,
                Key.M,
                Key.N,
                Key.O,
                Key.P,
                Key.Q,
                Key.R,
                Key.S,
                Key.T,
                Key.U,
                Key.V,
                Key.W,
                Key.X,
                Key.Y,
                Key.Z,
                Key.D0,
                Key.D1,
                Key.D2,
                Key.D3,
                Key.D4,
                Key.D5,
                Key.D6,
                Key.D7,
                Key.D8,
                Key.D9,
                Key.NumPad0,
                Key.NumPad1,
                Key.NumPad2,
                Key.NumPad3,
                Key.NumPad4,
                Key.NumPad5,
                Key.NumPad6,
                Key.NumPad7,
                Key.NumPad8,
                Key.NumPad9,
            };

        #endregion Private Fields

        public KeyboardTests()
        {
            Init();           
        }

        public void Dispose()
        {
            CleanUp();
        }

        public void Init()
        {
            // start notepad            
            _process = Process.Start("notepad.exe");
            _process.WaitForInputIdle();
        }

        public void CleanUp()
        {
            // close notepad
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
            }
        }

        [Fact]
        public void AlphaNumericPressReleaseTest()
        {
            // clear the clipboard 
            Clipboard.Clear();

            foreach (Key key in _alphanumericKeys)
            {
                Keyboard.Press(key);
                Keyboard.Release(key);
            }

            PostTextToClipboard();
            VerifyClipboardData(_alphanumericKeys);
        }

        [Fact]
        public void AlphaNumericTypeTest()
        {
            // clear the clipboard 
            Clipboard.Clear();

            foreach (Key key in _alphanumericKeys)
            {
                Keyboard.Type(key);                
            }

            PostTextToClipboard();
            VerifyClipboardData(_alphanumericKeys);
        }

        [Fact]
        public void TypeTest()
        {
            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("This is a test. *+-/;?~[|]\"\\");

            PostTextToClipboard();
            VerifyClipboardData("This is a test. *+-/;?~[|]\"\\");        
        }

        [Fact]
        public void EnterKeyTest()
        {
            Debug.WriteLine("Testing Enter key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Enter test.");
            Keyboard.Type(Key.Enter);
            Keyboard.Type("Enter test.");

            PostTextToClipboard();
            VerifyClipboardData("Enter test.\r\nEnter test.");          
        }

        [Fact]
        public void BackspaceKeyTest()
        {
            Debug.WriteLine("Testing Backspace key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Backspace test.");
            Keyboard.Type(Key.Back);
            Keyboard.Type(Key.Back);

            PostTextToClipboard();
            VerifyClipboardData("Backspace tes");        
        }

        [Fact]
        public void HomeAndDeleteKeyTest()
        {
            Debug.WriteLine("Testing Home and Delete key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Home and Delete test.");
            Keyboard.Type(Key.Home);
            Keyboard.Type(Key.Delete);
            Keyboard.Type(Key.Delete);

            PostTextToClipboard();
            VerifyClipboardData("me and Delete test.");          
        }

        [Fact]
        public void LeftRightKeyTest()
        {
            Debug.WriteLine("Testing Left and Right key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Left and Right test.");
            Keyboard.Type(Key.Left);
            Keyboard.Type(Key.Left);
            Keyboard.Type(Key.Delete);
            Keyboard.Type(Key.Right);
            Keyboard.Type(Key.Back);

            PostTextToClipboard();
            VerifyClipboardData("Left and Right tes");
        }

        [Fact]
        public void TabKeyTest()
        {
            Debug.WriteLine("Testing Tab key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Tab test.");
            Keyboard.Type(Key.Home);
            Keyboard.Type(Key.Tab);            

            PostTextToClipboard();
            VerifyClipboardData("\tTab test.");
        }

        [Fact]
        public void ShiftAndCapitalKeyTest()
        {
            Debug.WriteLine("Testing Shift key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Press(Key.Shift);
            Keyboard.Press(Key.V);
            Keyboard.Release(Key.V);
            Keyboard.Release(Key.Shift);

            PostTextToClipboard();
            VerifyClipboardData("V");

            Debug.WriteLine("Testing Capital key");

            try
            {
                // clear the clipboard 
                Clipboard.Clear();

                Keyboard.Type(Key.Capital);
                Keyboard.Type("this is a test");

                PostTextToClipboard();
                VerifyClipboardData("THIS IS A TEST");
            }
            finally
            {
                Keyboard.Type(Key.Capital);
            }
        }

        [Fact]
        public void EndKeyTest()
        {
            Debug.WriteLine("Testing End key");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("End test.");
            Keyboard.Type(Key.Home);
            Keyboard.Type(Key.End);
            Keyboard.Type(Key.Back);

            PostTextToClipboard();
            VerifyClipboardData("End test");
        }

        [Fact]
        public void UpDownKeyTest()
        {
            Debug.WriteLine("Testing Up Down keys");

            // clear the clipboard 
            Clipboard.Clear();

            Keyboard.Type("Up Down test.");
            Keyboard.Type(Key.Enter);
            Keyboard.Type("Up Down test.");
            Keyboard.Type(Key.Up);
            Keyboard.Type(Key.Back);
            Keyboard.Type(Key.Down);
            Keyboard.Type(Key.Back);

            PostTextToClipboard();
            VerifyClipboardData("Up Down test\r\nUp Down tes.");
        }

        #region Private Methods

        private void PostTextToClipboard()
        {
            // select all the text
            Keyboard.Press(Key.Ctrl);
            Keyboard.Press(Key.A);
            Keyboard.Release(Key.A);
            Keyboard.Release(Key.Ctrl);
            Thread.Sleep(100);

            // copy the text to the clipboard
            Keyboard.Press(Key.Ctrl);
            Keyboard.Press(Key.C);
            Keyboard.Release(Key.C);
            Keyboard.Release(Key.Ctrl);
            Thread.Sleep(100);
        }

        private void VerifyClipboardData(Key[] expectedKeys)
        {           
            // read from the clipboard to verify
            var clipboardData = GetClipboardDataObject();
            var rawData = clipboardData.GetData(DataFormats.Text) as string;

            Assert.Equal(expectedKeys.Length, rawData.Length);

            for (int i = 0; i < rawData.Length; i++)
            {
                var actual = rawData[i].ToString().ToLower();
                var expected = expectedKeys[i].ToString().ToLower();

                if (expected.Contains("d") && expected.Length == 2)
                {
                    expected = expected.Replace("d", "");
                }
                else if (expected.Contains("numpad"))
                {
                    expected = expected.Replace("numpad", "");
                }

                Assert.Equal(expected, actual);
            }            
        }

        private void VerifyClipboardData(string expectedString)
        {            
            // read from the clipboard to verify
            var clipboardData = GetClipboardDataObject();
            var rawData = clipboardData.GetData(DataFormats.Text) as string;

            Assert.Equal(expectedString.Length, rawData.Length);

            for (int i = 0; i < rawData.Length; i++)
            {
                var actual = rawData[i].ToString();
                var expected = expectedString[i].ToString();
                Assert.Equal(expected, actual);
            }           
        }

        private IDataObject GetClipboardDataObject() 
        { 
            //  "CLIPBRD_E_CANT_OPEN" can be thrown here without warning.  Common solution says to
            //  just query for it again.  
            for (int i = 0; i < 20; i++) 
            { 
                try 
                { 
                    return Clipboard.GetDataObject(); 
                } 
                catch (System.Runtime.InteropServices.COMException)
                { 
                    // Nothing
                }
 
                System.Threading.Thread.Sleep(300); 
            } 

            return null;
        }

        #endregion Private Methods      
    }
}

