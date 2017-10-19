// Generated from statement.g4 by ANTLR 4.5.3
// jshint ignore: start
var antlr4 = require('antlr4/index');

// This class defines a complete listener for a parse tree produced by statementParser.
function statementListener() {
	antlr4.tree.ParseTreeListener.call(this);
	return this;
}

statementListener.prototype = Object.create(antlr4.tree.ParseTreeListener.prototype);
statementListener.prototype.constructor = statementListener;

// Enter a parse tree produced by statementParser#statement.
statementListener.prototype.enterStatement = function(ctx) {
};

// Exit a parse tree produced by statementParser#statement.
statementListener.prototype.exitStatement = function(ctx) {
};


// Enter a parse tree produced by statementParser#group.
statementListener.prototype.enterGroup = function(ctx) {
};

// Exit a parse tree produced by statementParser#group.
statementListener.prototype.exitGroup = function(ctx) {
};


// Enter a parse tree produced by statementParser#sequence.
statementListener.prototype.enterSequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#sequence.
statementListener.prototype.exitSequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#or_sequence.
statementListener.prototype.enterOr_sequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#or_sequence.
statementListener.prototype.exitOr_sequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#and_sequence.
statementListener.prototype.enterAnd_sequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#and_sequence.
statementListener.prototype.exitAnd_sequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#optional_sequence.
statementListener.prototype.enterOptional_sequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#optional_sequence.
statementListener.prototype.exitOptional_sequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#required_sequence.
statementListener.prototype.enterRequired_sequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#required_sequence.
statementListener.prototype.exitRequired_sequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#optional_repeated_sequence.
statementListener.prototype.enterOptional_repeated_sequence = function(ctx) {
};

// Exit a parse tree produced by statementParser#optional_repeated_sequence.
statementListener.prototype.exitOptional_repeated_sequence = function(ctx) {
};


// Enter a parse tree produced by statementParser#token.
statementListener.prototype.enterToken = function(ctx) {
};

// Exit a parse tree produced by statementParser#token.
statementListener.prototype.exitToken = function(ctx) {
};


// Enter a parse tree produced by statementParser#repeat.
statementListener.prototype.enterRepeat = function(ctx) {
};

// Exit a parse tree produced by statementParser#repeat.
statementListener.prototype.exitRepeat = function(ctx) {
};



exports.statementListener = statementListener;