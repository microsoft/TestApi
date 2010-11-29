@echo off
setlocal

pushd .
set TESTS_PATH=%~dp0
set XUNIT_PATH="..\Tools\xUnit"
set TAC_RESULTS=TestApiCoreAcceptanceTests.html
set TAW_RESULTS=TestApiWpfAcceptanceTests.html


echo.
echo.
echo.
echo This script is about to run all acceptance tests for TestApi.
echo It will change the current directory to %TESTS_PATH%
echo.
echo Interracting with the PC during test execution MAY AFFECT the test results. 
echo.
echo The test results will be saved in the following files:
echo.
echo   %TAC_RESULTS%
echo   %TAW_RESULTS%
echo.
echo.
pause

cd /d %TESTS_PATH%


echo.
echo Running tests for TestApiCore.dll
%XUNIT_PATH%\xunit.console.exe %TESTS_PATH%\TestApiCoreAcceptanceTests.dll /html %TAC_RESULTS%

echo.
echo Running tests for TestApiWpf.dll
%XUNIT_PATH%\xunit.console.exe %TESTS_PATH%\TestApiWpfAcceptanceTests.dll /html %TAW_RESULTS%


echo.
echo Done.
echo.
endlocal



