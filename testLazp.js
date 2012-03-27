
/*
Copyright (C) 2012, Bill Burdick, Tiny Concepts: http://tinyconcepts.com/fs.pl/lambda.fsl

(licensed with ZLIB license)

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software. If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.

3. This notice may not be removed or altered from any source distribution.
*/

/*
Tests for Lazp
*/

(function() {
  var LZ, T, U, assertEq, assertEval, assertParse, line, run, _i, _len, _ref, _ref2;

  U = require('util');

  LZ = require('./lazp.js');

  _ref = T = require('./testing.js'), run = _ref.run, assertParse = _ref.assertParse, assertEval = _ref.assertEval, assertEq = _ref.assertEq;

  console.log('Testing');

  _ref2 = "true a b = a\nfalse a b = b\nand a b = a b false\nor a b = a true b\ncons a b = \\f . f a b\nnil = \\x y . y\nhead l = l \\h t . h\ntail l = l \\h t . t\nnull l = l (\\h t D . false) true\nlast l = l (\\h t D . null t h (last t)) nil".split('\n');
  for (_i = 0, _len = _ref2.length; _i < _len; _i++) {
    line = _ref2[_i];
    LZ.evalLine(line);
  }

  run('test1', function() {
    return assertParse("\\x.x x y", "lambda x . apply (apply (ref x) (ref x)) (lit y)", "\\x.x x y");
  });

  run('test2', function() {
    return assertEval("(\\x . x) hello", 'hello');
  });

  run('test3', function() {
    return assertEval("eval (_apply (_lambda x (_ref x)) (_lit hello))", 'hello');
  });

  run('test4', function() {
    return assertEval("(_eq cons cons) yes no", 'yes');
  });

  run('test5', function() {
    return assertEval("(_eq cons true) yes no", 'no');
  });

  run('test6', function() {
    return LZ.eval(LZ.gen(LZ.parse("cons 1 2")));
  });

  run('test7', function() {
    return LZ.eval(LZ.gen(LZ.parse("head (cons 1 2)")));
  });

  run('test8', function() {
    return assertEval("head (cons 1 2)", '1');
  });

  run('test8', function() {
    return assertEval("tail (cons 1 2)", '2');
  });

  run('test8', function() {
    return assertEval("last (cons a nil)", 'a');
  });

  run('test8', function() {
    return assertEval("last (cons a (cons b nil))", 'b');
  });

  console.log('\nDone');

  if (!T.stats.failures) {
    console.log("Succeeded all " + T.stats.successes + " tests.");
  } else {
    console.log("Succeeded " + T.stats.successes + " test" + (T.stats.successes > 1 ? 's' : '') + "\nFailed " + T.stats.failures + " test" + (T.stats.failures > 1 ? 's' : ''));
  }

}).call(this);