[Go Up](./)

# Indentation and Spacing
* Tabs are to be used instead of spaces.
* Blank lines can be used to separate groups of related code, such as:
	* Class members and instance members.
	* Public and protected/private variables.
	* Class/instance members and methods.
	* Between clusters of methods that are not tightly related.
	* Blocks of code within a method that have distinct purpose, but don't already belong in a switch statement.
		* Consider using helper methods for readability purposes, even if there is only one call site.
* Spaces should be avoided except as the language demands it, or when readability demands it.
	* It is not necessary to put spaces after commas, or around operators.
	* Source code files should not end in a newline. They are not POSIX files.
	* Unity adds a byte-order mark to plaintext files on creation. Remove it with a hex editor or CLI tool before committing.
		* Files created manually and then auto-detected by the editor do not have this problem.
	* Exception: Assignment statements have spaces around the = operator.
* Excessively long lines can be manually wrapped at the developer's discretion.
	* If multiple lines are part of the same statement, lines after the first should have one additional intendation level.
* Opening and closing syntax should be vertically aligned if they span multiple lines.
	* This applies to braces, parentheses, and brackets.
	* Not necessary if the expression/block fits into one line.
* Multiple statements cannot share the same line, except branch statements and a one-statement code block.
* No code should be aligned except at the beginning of a line.
	* If parts of code should be aligned, such as parameter lists, but some are on a new line, and some share their line with the beginning of the statement, put the first such part on a new line instead of only the ones that run off the screen.
* One-statement code blocks and no-ops may share the line of the switch statement or method signature, but only if this would not make the line unwieldy.
	* Curly braces are required for all code blocks.
* Class names and method names require word-capitalization.
	* This is to match the style used by .NET and Unity.
* Member variables and local variables require camel case.
* Constants require snake case.
* All method and variable names should be descriptive.
	* If you can't think of a good name, you don't understand the problem well enough.
	* Exception: Nondescriptive but industry-standard names, such as i for for loops, or letter variables from famous math equations, such as r in PIr^2.
* Automatically-generated files should be modified as litle as possible, whether they are binary or plaintext.
	* These files can violate all other rules in this list.
	* This rule prevents hand-made modifications from being overwritten.

## Examples
```C#
//Bad: Parameters not aligned; Parentheses not aligned.
UtilityClass.StaticMethod(parameter1,reallylongExpression,
	anotherExpressionThatDidntFitUpThere);
```
```C#
//Good
UtilityClass.StaticMethod
(
	parameter1,
	reallyLongExpression,
	anotherExpressionThatDidntFitUpThere
);
```
```C#
//Bad: Mixed tabs and spaces; Code aligned elsewhere than beginning of line.
//(The beginning of the line is after the tabs, not the space.)
class TestingClass
{
	/**
	 * Javadoc style method description.
	 *
	 * <- Space after tab! Bad!
	 */
	public static void blah(){}
}
```
```C#
//Good
class TestingClass
{
	/*
	* The double-asterisk isn't needed since Javadoc syntax isn't recognized in the C# world.
	* Microsoft wants us to use XML instead. *Screw that.*
	*/
	public static void blah(){}
}
```
```C#
//Good: All valid styles.
public static void blah(){shortFunctionName();}

public static void noop(){}

public static void blah()
{
	//Collapsing one-liners is optional.
	shortFunctionName();
}
```