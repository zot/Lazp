[Main](README.html) [Reference](REFERENCE.html) [Status](TODO.html) [Source](http://github.com/zot/Leisure) [REPL](leisure.html) [Standard functions](std.lsr)
# Please Note: the repository is now [http://github.com/zot/Leisure](http://github.com/zot/Leisure)

# Leisure: a convenient, untyped, lazy Lambda Calculus with Metaprogramming and Primitives

The goal, here is to provide a convenient language people's use and experimentation that people can easily tailor to their own needs.

## Convenient
Leisure provides a convenient syntax and powerful tools to help people try things out quickly and be productive.  Leisure's syntax is based on Lambda Calculus and borrows things from Haskell and other languages while still remaining a very small language.

### Lambdas
As in Haskell, you can write lambda either as 𝛌 or \.  Lambdas are structured as: **𝛌 variable . body** and if you provide serveral space-separated variables, Leisure automatically constructs nested lambdas for you, like this: **\a b . a**

### Function definitions
You define a function with a declaration, an '=', and a body, like this: **true = \a b . a**.  You maplace arguments before the '=' and omit the '\' and '.', like this: **true a b = a**.

Leisure allows some simplistic parser tweaks; you can define tokens, so you can leave out spaces, and you can define tokens that open and close groups.

### Curly braces
Curly braces produce in-line groups, separated by semicolons, so instead of **(a (b c) (d e f) g)**, you could say: **a {b c; d e f} g** (or **{a;b c;d e f;g}**, etc.).

### Indentation
Leisure supports Python-style indentation, which it replaces with curly braces and semicolons during parsing.  Indented lines are considered to be part of the preceding overhanging line and each indented line produces a parenthesized group, so this:

<pre><b><big>
do  
  n <- prompt "What is your name? "  
  a <- prompt (concat ["Hello, ", n, ", thank you for joining us.  How old are you?"])
  seconds = * a 31536000
  print (concat ["You have been alive more than ", seconds, " seconds."])
</big></b></pre>

is equivalent to this: **do {n <- prompt "What is your name? "; print (concat ["Hello, ", n, ", thank you for joining us."])}**

and this:

<pre><b><big>
let
  a = 3
  b = + a 1
  a = * a b
  [a, b]
</big></b></pre>

is equivalent to this: **let {a <- 3; b <- + a 1; a <- * a b; [a, b]}**

## Untyped
Leisure is untyped.  This doesn't mean that Leisure is type-free, it just means that variables don't have types -- they can hold anything.  Leisure does have data types -- e.g. you can tell if something is a cons-cell (**_is (cons 1 nil) cons** returns **true**).  Whenever you define a function that returns a lambda, it marks instances of that lambda as having the type of the definition.  For example, if you say:

**person name addr = \f . f name addr**

Then **(is (person a b) person)** will return **true** and **(getType (person a b))** will return **some person** (an option).  **(getType \x . x)**, however, will return none, because naked lambdas have no type (at this point, anyway).

## Lazy
Leisure is lazy.  It doesn't allow side effects, not because side effects are somehow *evil*, but because in a lazy language, side effects can lead to very strange behavior that's very hard to diagnose.  The idea is to have side effects outside the Leisure environment and access them using standard functional techniques, like monads and FRP.

## Metaprogramming
Leisure provides several tools for metaprogramming:

Functions
* getType data -- returns an option with the type of the data (or none if it has no type)
* is data type -- returns a boolean indicating whether data is of the type
* eq a b -- returns a boolean indicating whether a and b are identically equal
* parse string -- returns an AST for the string
* eval AST -- evaluates ast

Macros
* declaration =M= definition -- declares a macro that runs at parse-time.  Input is the AST for the macro expression and output is a new AST to replace the old one.  Further macro replacement is done on the result.  The identity macro is: **identMacro ast =M= ast**.
* Do and let are macros
   * let uses = to assign (and reassign) variables and returns the result of an expression using those variables
   * do uses = like let and uses <- to bind monads

## AST Function usage
lit v -- a literal value  
ref v -- a variable reference: v should be a string  
lambda var body -- a lambda: var should be a string or number, body is an AST  
apply func arg -- a function application: func can be any AST function except a _prim  

## AST Function definitions

lit x = \f . f x  
ref x = \f . f x  
lambda v f = \g . g v f  
apply func arg = \f . f func arg  

## Running it
### Running the REPL
>node runRepl
Runs the read eval print loop.  You can enter Leisure expressions and definitions there.

### Compiling files
>node runRepl -c file1 file2 ...
Compiles files.  

### Running the tests
To run the tests, you can cd into the top level directory and type
> node testLeisure
It should give you something like this:
> Running Tests...
> ...
> Done
> Succeeded all 38 tests.
Each . on the second line represents a successfully completed test.  If there are errors, it will tell you which tests failed.

## Changing it
If you want to change things, you'll probably want to change the Coffeescript files, rather than their corresponding javascript versions.  The [build](build) file that contains a command to rebuild it, which is just
> node_modules/coffee-script/bin/coffee -c leisure.cs repl.cs runRepl.cs

## Implementation
Leisure is written in JavaScript and runs in node or in browsers.  Later, Leisure may generate native code, using, for instance, C and/or LLVM.  An LLVM version could use the VMKit’s garbage collector or the Boehm-Demers-Weiser garbage collector, which performs well with small objects (like function contexts).
