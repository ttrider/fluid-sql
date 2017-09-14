// Generated from statement.g4 by ANTLR 4.5.3
// jshint ignore: start
var antlr4 = require('antlr4/index');

// This class defines a complete generic visitor for a parse tree produced by statementParser.

function statementVisitor() {
	antlr4.tree.ParseTreeVisitor.call(this);
	return this;
}

statementVisitor.prototype = Object.create(antlr4.tree.ParseTreeVisitor.prototype);
statementVisitor.prototype.constructor = statementVisitor;

// Visit a parse tree produced by statementParser#statement.
statementVisitor.prototype.visitStatement = function(ctx) {
};


// Visit a parse tree produced by statementParser#group.
statementVisitor.prototype.visitGroup = function(ctx) {
};


// Visit a parse tree produced by statementParser#sequence.
statementVisitor.prototype.visitSequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#or_sequence.
statementVisitor.prototype.visitOr_sequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#and_sequence.
statementVisitor.prototype.visitAnd_sequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#optional_sequence.
statementVisitor.prototype.visitOptional_sequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#required_sequence.
statementVisitor.prototype.visitRequired_sequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#optional_repeated_sequence.
statementVisitor.prototype.visitOptional_repeated_sequence = function(ctx) {
};


// Visit a parse tree produced by statementParser#token.
statementVisitor.prototype.visitToken = function(ctx) {
};


// Visit a parse tree produced by statementParser#repeat.
statementVisitor.prototype.visitRepeat = function(ctx) {
};



exports.statementVisitor = statementVisitor;