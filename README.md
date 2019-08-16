# TestApi is...
TestApi is a library of *test and utility APIs* that enables developers and testers to create testing tools and automated tests for .NET and Win32 applications. TestApi provides a set of common test building blocks -- types, data-structures and algorithms -- in a simple, layered, componentized and documented stack.

# Get started
* Get the [latest release](http://www.codeplex.com/TestApi/Release/ProjectReleases.aspx).
* Read the following blog articles for a quick introduction to some of the available features:  
  * [Overview of TestApi](http://blogs.msdn.com/ivo_manolov/archive/2009/10/14/9907447.aspx)
  * [Part 1: Input Injection APIs](http://blogs.msdn.com/ivo_manolov/archive/2008/12/15/9223397.aspx)
  * [Part 2: Command-Line Parsing APIs](http://blogs.msdn.com/ivo_manolov/archive/2008/12/17/9230331.aspx)
  * [Part 3: Visual Verification APIs](http://blogs.msdn.com/ivo_manolov/archive/2009/04/20/9557563.aspx)
  * [Part 4: Combinatorial Variation Generation APIs](http://blogs.msdn.com/ivo_manolov/archive/2009/08/26/9884004.aspx)
  * [Part 5: Managed Code Fault Injection APIs](http://blogs.msdn.com/ivo_manolov/archive/2009/11/25/9928447.aspx)
  * [Part 6: Text String Generation APIs](http://blogs.msdn.com/ivo_manolov/archive/2010/04/14/9995847.aspx)
  * [Part 7: Memory Leak Detection APIs](http://blogs.msdn.com/ivo_manolov/archive/2010/04/14/9995880.aspx)
  * [Part 8: Object Comparison APIs](http://blogs.msdn.com/b/ivo_manolov/archive/2010/07/29/10043968.aspx)
  * Part 9: Application Control APIs
* Read the documentation, experiment with the samples, check out the source code.
* Let us know what features you would like to see in future releases.

# Vote
Please [vote on http://testapi.uservoice.com](http://testapi.uservoice.com) for specific features, or just [send us feedback](http://www.codeplex.com/TestApi/Thread/List.aspx) on the usefulness, architecture, and relevance of the provided APIs, and let us know what APIs you need for your work. The library will evolve based on user feedback.

# Roadmap
* Existing APIs
  * _Application control API_
  * _Combinatorial variation generation API_
  * _Commandline parsing API_ 
  * _Input API_
  * _Leak detection API_ 
  * _Managed code fault injection API_
  * _Object comparison API_ 
  * _Text string generation API_ 
  * _Theme API_
  * _UIA utility API_
  * _Visual verification API_
  * _WPF dispatcher operations API_

* Suggested Future APIs
  * _Concurrency API_
  * _Controls verifications API_
  * _Cross-platform support_
  * _Fuzzing API_
  * _Input injection API (additions)_
  * _Media verification API_ 
  * _Mocking API_  
  * _Performance API_
  * _Screen resolution API_
  * _Setup validation API_
  * _State management API_
  * _Stress and load testing API_ 
  * _UIA utility API (additions)_ 
  * _Unmanaged code fault injection API_ 
  * _Unmanaged API interface_ 

* Package Components
  * _Acceptance tests_ 
  * _Binaries_
  * _Documentation_
  * _MSTest samples_
  * _Sources_
  * _xUnit and NUnit samples_

# Contributors
Project Development:
* Alexis Roosa
* Andrey Arkharov
* Anne Gao
* Bill Liu ([blog](http://blogs.msdn.com/b/billliu))
* Daniel Marley
* Dennis Deng
* Eddie Li 
* Eugene Zakhareyev (CARBON!)
* Ivo Manolov ([blog](http://blogs.msdn.com/ivo_manolov))
* Jared Moore
* Nathan Anderson
* Peter Antal ([blog](http://blogs.msdn.com/pantal/default.aspx))
* Ranjesh Jaganathan
* Sam Terilli
* Shozub Qureshi
* Tim Cowley
* Vincent Sibal ([blog](http://blogs.msdn.com/vinsibal/default.aspx))
* William Han

Reviewers:  
* Abhishek Kumar Mishra, Adam Ulrich, Adrian Vinca, Alexis Roosa, Brad Van Ee, Brian McMaster, Christine Warren, Daniel Marley, Dawn Wood, Dennis Cheng, Dwayne Need, Krasimir Alexandrov, Ian Ellison-Taylor, Jim Galasyn, John Gossman ([[blog](http://blogs.msdn.com/johngossman)), Lester Lobo ([blog](http://blogs.msdn.com/llobo)), Mak Agashe, Matt Galbraith, Michael Hunter ([blog](http://blogs.msdn.com/micahel)), Mike Pope, Patrick Danino ([blog](http://blogs.msdn.com/PatrickDanino), Peter Antal, Robert Lyon, Rossen Atanassov, Scott Shigeta, Scott Wadsworth, Sue Dernbach, Yong Lee.


# Part 1: Input Injection APIs

I am starting a series of posts introducing some of the facilities available in TestApi, a test and utility API library, which we recently released on CodePlex. Most of this content is already available in the documentation provided with the library.

The first post is on input injection – a fairly common activity in UI testing.

 

General Notes
Input injection is the act of simulating user input. In general, there are several ways to simulate user input, in the following progressively increasing levels of realism:

Direct method invocation: A test programmatically triggers events by directly calling methods on the target UI element. For example, a test can call the Button.IsPressed method to simulate pressing a WPF button.
Invocation using an accessibility interface (UIA, MSAA, etc.): A test programmatically triggers events by calling methods on an AutomationElement instance that represents the target UI element.
Simulation using low-level input: A test simulates input by using low-level input facilities provided by the host operating system. Examples of such facilities on Windows are the SendInput Win32 API and the Raw Input Win32 API, which inject input directly into the OS input stream.
Simulation using a device driver: A test uses a device driver to simulate input at the device-driver level.
Simulation using a robot: A test controls a robot to simulate direct human interaction with an input device (for example, pressing keys on a keyboard).
Technique A is framework-specific; what works for WPF does not work for Windows Forms and vice versa. Technique B is less framework-specific than A, but still has limitations, because some frameworks differ in their implementations of the required accessibility interfaces. Techniques C and D are OS-specific. Technique D is significantly more difficult to implement and deploy than C, without a corresponding increase in its level of realism. Technique E is universal, albeit much slower and much more expensive than the other options.

The TestApi library provides facilities both for B (through the AutomationUtilities class) and for C (through the Mouse and Keyboard classes), which are the most generally useful techniques of input simulation.

 

 

Examples
The AutomationUtilities class provides wrappers for common UIA operations, such as discovery of UI elements. The first example below demonstrates how to discover and click a WPF Button in a WPF Window, by using the AutomationUtilities class and the Mouse class.

    //
    // EXAMPLE #1
    // This code below discovers and clicks the Close button in an About dialog box, thus
    // dismissing the About dialog box.
    //

    string aboutDialogName = "About";
    string closeButtonName = "Close";

    AutomationElementCollection aboutDialogs = AutomationUtilities.FindElementsByName(
        AutomationElement.RootElement,
        aboutDialogName);

    AutomationElementCollection closeButtons = AutomationUtilities.FindElementsByName(
        aboutDialogs[0],
        closeButtonName);

    //
    // You can either invoke the discovered control, through its invoke pattern...
    //

    InvokePattern p = 
        closeButtons[0].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
    p.Invoke();

    //
    // ... or you can handle the Mouse directly and click on the control.
    //

    Mouse.MoveTo(closeButton.GetClickablePoint());
    Mouse.Click(MouseButton.Left);
 

The second example below demonstrates how to discover a TextBox instance and type in it, using the Mouse and Keyboard classes as wrappers of common mouse and keyboard operations:

    //
    // EXAMPLE #2
    // Discover the location of a TextBox with a given name.
    //

    string textboxName = "ssnInputField";

    AutomationElement textBox = AutomationUtilities.FindElementsByName(
        AutomationElement.RootElement,
        textboxName)[0];

    Point textboxLocation = textbox.GetClickablePoint();

    //
    // Move the mouse to the textbox, click, then type something
    //

    Mouse.MoveTo(textboxLocation);
    Mouse.Click(MouseButton.Left);

    Keyboard.Type("Hello world.");
    Keyboard.Press(Key.Shift);
    Keyboard.Type("hello, capitalized world.");
    Keyboard.Release(Key.Shift);


In Conclusion
The Mouse and Keyboard classes in the TestApi library can be used for automating of any application, running on Windows. The classes are completely policy- and context-free – their usage is not dependent on a specific test framework or on a specific test workflow. TestApi provides full source code and XML documentation for these classes, so you can either integrate them in your own projects or reference the pre-built DLLs.

Note that, even though Mouse, Keyboard and AutomationUtilities make the life of the test automation developer quite a bit easier, UI testing is tricky and should be avoided whenever possible. It’s always preferable to design your application as a multi-tier application, with a “thin” UI layer, so that you can bypass the UI in most of your tests.
