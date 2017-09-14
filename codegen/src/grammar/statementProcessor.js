var antlr4 = require("antlr4");
var statementLexer = require("./statementLexer");
var statementParser = require("./statementParser");
var statementListener = require("./statementListener");

// var KeyPrinter = function() {
//     statementListener.call(this); // inherit default listener
//     return this;
// };

// Rachel Maas

// // inherit default listener
// KeyPrinter.prototype = Object.create(statementListener.prototype);
// KeyPrinter.prototype.constructor = KeyPrinter;

// KeyPrinter.prototype.enterToken = function(ctx) {
//     console.log("token");
// };

// exports.KeyPrinter = KeyPrinter;