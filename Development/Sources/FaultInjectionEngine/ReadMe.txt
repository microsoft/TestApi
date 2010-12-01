IMPORTANT:

NOTE 1:
The directories FaultInjectionEngine\x86 and FaultInjectionEngine\x64 
contain important runtime dependencies for FaultInjectionEngine.dll.

You may need to copy these dependencies in the directory of FaultInjectionEngine.dll
if the Fault Injection API fail to work as expected. In most cases, however, you 
won't need to copy these dependencies.

NOTE 2:
In order to build FaultInjectionEngine.dll, you need to edit the following files:

    DllData.c
    FaultInjectionEngine.h
    FaultInjectionEngine_i.c
    FaultInjectionEngine_p.c

We will be fixing this soon.
